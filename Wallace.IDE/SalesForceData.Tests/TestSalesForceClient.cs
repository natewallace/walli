using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SalesForceData.Tests
{
    /// <summary>
    /// Tests for the SalesForceClient.
    /// </summary>
    [TestClass]
    public class TestSalesForceClient
    {
        #region Methods

        /// <summary>
        /// Test the DescribeGlobal method.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestDataDescribeGlobal()
        {
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SObjectTypePartial[] partialObjectDescriptions = client.DataDescribeGlobal();
                foreach (SObjectTypePartial partialObjectDescription in partialObjectDescriptions)
                    Console.WriteLine(partialObjectDescription.Name);
            }
        }

        /// <summary>
        /// Test the DataDescribeObjectType method.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestDataDescribeObjectType()
        {
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SObjectTypePartial[] partialObjectDescriptions = client.DataDescribeGlobal();
                SObjectTypePartial partialObjectDescription = partialObjectDescriptions
                                                              .Where(pod => pod.Name == "Branch__c")
                                                              .First();

                SObjectType objectDescription = client.DataDescribeObjectType(partialObjectDescription);
                Console.WriteLine(objectDescription.Name);
                foreach (SObjectFieldType fieldDescription in objectDescription.Fields)
                    Console.WriteLine("   " + fieldDescription.Name);
            }
        }

        /// <summary>
        /// Test the DataSelect method.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestDataSelect()
        {
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SObjectTypePartial[] partialObjectDescriptions = client.DataDescribeGlobal();
                SObjectTypePartial partialObjectDescription = partialObjectDescriptions
                                                              .Where(pod => pod.Name == "Branch__c")
                                                              .First();

                SObjectType objectDescription = client.DataDescribeObjectType(partialObjectDescription);
                List<SObjectFieldType> insertableFields = objectDescription.Fields
                                                          .Where(f => !f.AutoNumber && 
                                                                      !f.Calculated && 
                                                                      !f.DeprecatedAndHidden && 
                                                                      f.Createable)
                                                          .ToList();

                StringBuilder query = new StringBuilder();
                query.AppendFormat("SELECT {0} FROM {1}", String.Join(",", insertableFields), objectDescription.Name);

                DataSelectResult result = client.DataSelect(query.ToString());

                StringBuilder output = new StringBuilder();
                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings()
                {
                    ConformanceLevel = System.Xml.ConformanceLevel.Fragment,
                    Indent = true,
                    IndentChars = String.Empty
                };

                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(output, settings))
                {
                    writer.WriteStartElement("table");
                    writer.WriteAttributeString("type", objectDescription.Name);
                    
                    foreach (DataRow row in result.Data.Rows)
                    {
                        writer.WriteStartElement("row");
                        for (int i = 0; i < result.Data.Columns.Count; i++)
                            writer.WriteAttributeString(result.Data.Columns[i].ColumnName, Convert.ToString(row[i]));

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                Console.Write(output.ToString());
            }
        }

        /// <summary>
        /// Test the DataInsert method.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestDataInsert()
        {
            Assert.IsTrue(false, "Test code is commented out.");
            /*
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                DataTable table = new DataTable("Branch__c");
                table.Columns.Add("Name", typeof(string));
                table.Columns.Add("Active__c", typeof(bool));
                table.Columns.Add("Test__c", typeof(int));

                DataRow row = table.NewRow();
                row["Name"] = "Test Branch " + DateTime.Now.ToFileTime();
                row["Active__c"] = false;
                row["Test__c"] = 99;
                table.Rows.Add(row);

                client.DataInsert(table);

                Console.WriteLine("Row id = " + row["Id"]);
            }
            */
        }

        /// <summary>
        /// Test the GetAllSourceFileTypes method.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestGetAllSourceFileTypes()
        {
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SourceFileType[] fileTypes = client.GetSourceFileTypes();
                Array.Sort(fileTypes);

                foreach (SourceFileType fileType in fileTypes)
                {
                    Console.WriteLine(fileType.Name);
                    foreach (SourceFileType child in fileType.Children)
                    {
                        Console.Write("\t");
                        Console.WriteLine(child.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Test the GetSourceFiles method.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestGetSourceFiles()
        {
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SourceFileType[] fileTypes = client.GetSourceFileTypes();
                SourceFile[] files = client.GetSourceFiles(fileTypes, true);
                files = files.OrderBy(f => f.FileType).ToArray();

                foreach (SourceFile file in files)
                {
                    Console.WriteLine(String.Format("{0} : {1}", file.FileType.Name, file.Name));
                    foreach (SourceFile child in file.Children)
                        Console.WriteLine(String.Format("    {0} : {1}", child.FileType.Name, child.Name));
                }
            }
        }

        /// <summary>
        /// Test the seralization methods.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestSerialization()
        {
            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;

            // test credential
            SalesForceCredential credential = new SalesForceCredential(
                SalesForceDomain.Sandbox,
                "testname",
                "testpassword",
                "testtoken");

            StringBuilder xml = new StringBuilder();
            System.Xml.XmlWriter credWriter = System.Xml.XmlWriter.Create(xml, settings);
            System.Xml.Serialization.XmlSerializer credSer = new System.Xml.Serialization.XmlSerializer(typeof(SalesForceCredential));
            credSer.Serialize(credWriter, credential);

            Console.WriteLine(xml.ToString());
            Console.WriteLine();

            // test source file 
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SourceFileType[] fileTypes = client.GetSourceFileTypes();
                SourceFile[] files = client.GetSourceFiles(fileTypes, true);
                
                StringBuilder output = new StringBuilder();                
                System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(output, settings);

                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(SourceFile[]));
                ser.Serialize(writer, files);

                Console.WriteLine(output.ToString());

                System.IO.StringReader reader = new System.IO.StringReader(output.ToString());
                SourceFile[] otherFiles = ser.Deserialize(reader) as SourceFile[];

                Assert.AreEqual(files.Length, otherFiles.Length);
                for (int i = 0; i < files.Length; i++)
                    Assert.AreEqual(files[i].Children.Length, otherFiles[i].Children.Length);
            }
        }

        /// <summary>
        /// Test to get reports.
        /// </summary>
        [TestMethod]
        public void SalesForceClient_TestReportDownload()
        {
            // test source file 
            using (SalesForceClient client = new SalesForceClient(Utility.TestCredential, Utility.SalesForceDataConfiguration))
            {
                SourceFileType reportFileType = client.GetSourceFileTypes().Where(sft => sft.Name == "Report").FirstOrDefault();
                SourceFile[] files = client.GetSourceFiles(new SourceFileType[] { reportFileType }, true);
                foreach (SourceFile file in files)
                    Console.WriteLine(file.Name);
            }
        }

        #endregion
    }
}

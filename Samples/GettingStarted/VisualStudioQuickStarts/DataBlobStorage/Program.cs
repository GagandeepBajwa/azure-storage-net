//----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//----------------------------------------------------------------------------------

namespace DataBlobStorageSample
{
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Azure Storage Blob Sample - Demonstrate how to use the Blob Storage service. 
    /// Blob storage stores unstructured data such as text, binary data, documents or media files. 
    /// Blobs can be accessed from anywhere in the world via HTTP or HTTPS.
    ///
    /// Note: This sample uses the .NET 4.5 asynchronous programming model to demonstrate how to call the Storage Service using the 
    /// storage client libraries asynchronous API's. When used in real applications this approach enables you to improve the 
    /// responsiveness of your application. Calls to the storage service are prefixed by the await keyword. 
    /// 
    /// Documentation References: 
    /// - What is a Storage Account - http://azure.microsoft.com/en-us/documentation/articles/storage-whatis-account/
    /// - Getting Started with Blobs - http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/
    /// - Blob Service Concepts - http://msdn.microsoft.com/en-us/library/dd179376.aspx 
    /// - Blob Service REST API - http://msdn.microsoft.com/en-us/library/dd135733.aspx
    /// - Blob Service C# API - http://go.microsoft.com/fwlink/?LinkID=398944
    /// - Delegating Access with Shared Access Signatures - http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-shared-access-signature-part-1/
    /// - Storage Emulator - http://msdn.microsoft.com/en-us/library/azure/hh403989.aspx
    /// - Asynchronous Programming with Async and Await  - http://msdn.microsoft.com/en-us/library/hh191443.aspx
    /// </summary>
    public class Program
    {
        // *************************************************************************************************************************
        // Instructions: This sample can be run using either the Azure Storage Emulator that installs as part of this SDK - or by
        // updating the App.Config file with your AccountName and Key. 
        // 
        // To run the sample using the Storage Emulator (default option)
        //      1. Start the Azure Storage Emulator (once only) by pressing the Start button or the Windows key and searching for it
        //         by typing "Azure Storage Emulator". Select it from the list of applications to start it.
        //      2. Set breakpoints and run the project using F10. 
        // 
        // To run the sample using the Storage Service
        //      1. Open the app.config file and comment out the connection string for the emulator (UseDevelopmentStorage=True) and
        //         uncomment the connection string for the storage service (AccountName=[]...)
        //      2. Create a Storage Account through the Azure Portal and provide your [AccountName] and [AccountKey] in 
        //         the App.Config file. See http://go.microsoft.com/fwlink/?LinkId=325277 for more information
        //      3. Set breakpoints and run the project using F10. 
        // 
        // *************************************************************************************************************************


        //connection string for the emulator cloud storage
       public static string connectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1; AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;";



        [STAThread]
        static void Main(string[] args)
        {
            int run = 0;

            while (run == 0)
            {


                Console.WriteLine("Azure Storage Blob Sample\n ");

                Console.WriteLine("Select your choice\n");

                Console.WriteLine("1. Upload data to the cloud Blob Storage");
                Console.WriteLine("2. List all the Blobs");
                Console.WriteLine("3. Download Blobs\n \n");
                Console.WriteLine("Enter 0 to exit");

                int choice = -1;

                //Take the input
                choice = Int32.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        UploadData();
                        break;

                    case 2:

                        break;

                    case 3:

                        break;

                }

                if (choice == 0)
                {
                    //break out of the apps infinite loop
                    break;
                }                
            }
        }


        ///<summary>
        ///Entry point function for the evrything related to uploding data
        /// </summary>
        private static void UploadData()
        {
            Console.WriteLine("Enter your Choice! (Like 1, 2) \n\n");

            Console.WriteLine("1.Upload Data as the BlockBlob");
            Console.WriteLine("2. Upload Data as the Page Blob\n\n");

            int choice = -1;

            //Take the input
            choice = Int32.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    BasicStorageBlockBlobOperationsAsync().Wait();
                    break;

                case 2:

                    break;

            }

        }








        /// <summary>
        /// Basic operations to work with block blobs
        /// </summary>
        /// <returns>Task<returns>
        private static async Task BasicStorageBlockBlobOperationsAsync()
        {
            //Testing image for the cloud bliob storage
            const string ImageToUpload = "HelloWorld.png";

            string[] FileNamesToUpload = null;

            //Choose the files to be uploaded
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                     FileNamesToUpload = openFileDialog.FileNames;
                }
                catch(SecurityException e)
                {
                    MessageBox.Show(e.ToString());
                }
            }

            // Retrieve storage account information from connection string
            // How to create a storage connection string - http://msdn.microsoft.com/en-us/library/azure/ee758697.aspx
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings.Get("StorageConnectionString"));
       
            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();


            Console.WriteLine("Processing your inputs");

            // Create a container for organizing blobs within the storage account.
            Console.WriteLine("\n 1. Creating Container");
            CloudBlobContainer container = blobClient.GetContainerReference("democontainerblockblob");
            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException)
            {
                Console.WriteLine("If you are running with the default configuration please make sure you have started the storage emulator. Press the Windows key and type Azure Storage to select and run it from the list of applications - then restart the sample.");
                Console.ReadLine(); 
                throw; 
            }

            // To view the uploaded blob in a browser, you have two options. The first option is to use a Shared Access Signature (SAS) token to delegate 
            // access to the resource. See the documentation links at the top for more information on SAS. The second approach is to set permissions 
            // to allow public access to blobs in this container. Uncomment the line below to use this approach. Then you can view the image 
            // using: https://[InsertYourStorageAccountNameHere].blob.core.windows.net/democontainer/HelloWorld.png
            // await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });

            // Upload a BlockBlob to the newly created container
            Console.WriteLine("2. Uploading BlockBlob");
            CloudBlockBlob blockBlob = null;
            foreach (string file in FileNamesToUpload)
            {
                blockBlob = container.GetBlockBlobReference(file);
                await blockBlob.UploadFromFileAsync(file, FileMode.Open);
            }


            //All the possible operations on the Blocklob
            Console.WriteLine("\n Type Your Choice");
            Console.WriteLine("1. List all the blobs");
            Console.WriteLine("2. Download the Blob");
            Console.WriteLine("3. Delete the Blob");


            int blobChoice = -1;
            blobChoice = Int32.Parse(Console.ReadLine());
            switch (blobChoice)
            {
                case 1:
                    Console.WriteLine("Listing Blobs in Container");
                    foreach (IListBlobItem blob in container.ListBlobs())
                    {
                        // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory
                        // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
                        Console.WriteLine("- {0} (type: {1})", blob.Uri, blob.GetType());
                    }
                    break;

                case 2:
                    Console.WriteLine("Listing Blobs in Container for download");
                    int index = 1;
                    foreach (IListBlobItem blob in container.ListBlobs())
                    {
                        // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory
                        // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
                        Console.WriteLine(index+". " + "- {0}   (type: {1})", blob.Uri, blob.GetType());

                    }

                    Console.WriteLine("Enter your slection");
                    int downloadChoice = -1;
                    downloadChoice = Int32.Parse(Console.ReadLine());


                    Console.WriteLine("Your Download in progress");

                    await blockBlob.DownloadToFileAsync(string.Format("./CopyOf{0}", FileNamesToUpload[downloadChoice] ), FileMode.Create);
                    break;


                case 3:
                    Console.WriteLine("Listing Blobs in Container to delete");
                    int deleteIndex = 1;
                    foreach (IListBlobItem blob in container.ListBlobs())
                    {
                        // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory
                        // Use blob.GetType() and cast to appropriate type to gain access to properties specific to each type
                        Console.WriteLine(deleteIndex + ". " + "- {0}   (type: {1})", blob.Uri, blob.GetType());

                    }

                    Console.WriteLine("Enter your slection");
                    int deleteChoice = -1;
                    deleteChoice = Int32.Parse(Console.ReadLine());

                    Console.WriteLine("Deletion in progress");
                    await blockBlob.DeleteAsync();


                    break;
            }

 

            // Clean up after the demo 
           

            // When you delete a container it could take several seconds before you can recreate a container with the same
            // name - hence to enable you to run the demo in quick succession the container is not deleted. If you want 
            // to delete the container uncomment the line of code below. 
            //Console.WriteLine("6. Delete Container");
            //await container.DeleteAsync();
        }

        /// <summary>
        /// Basic operations to work with page blobs
        /// </summary>
        /// <returns>Task</returns>
        private static async Task BasicStoragePageBlobOperationsAsync()
        {
            const string PageBlobName = "samplepageblob";

            // Retrieve storage account information from connection string
            // How to create a storage connection string - http://msdn.microsoft.com/en-us/library/azure/ee758697.aspx
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings.Get("StorageConnectionString"));

            // Create a blob client for interacting with the blob service.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Create a container for organizing blobs within the storage account.
            Console.WriteLine("1. Creating Container");
            CloudBlobContainer container = blobClient.GetContainerReference("democontainerpageblob");
            await container.CreateIfNotExistsAsync();

            // Create a page blob in the newly created container.  
            Console.WriteLine("2. Creating Page Blob");
            CloudPageBlob pageBlob = container.GetPageBlobReference(PageBlobName);
            await pageBlob.CreateAsync(512 * 2 /*size*/); // size needs to be multiple of 512 bytes

            // Write to a page blob 
            Console.WriteLine("2. Write to a Page Blob");
            byte[] samplePagedata = new byte[512];
            Random random = new Random();
            random.NextBytes(samplePagedata);
            await pageBlob.UploadFromByteArrayAsync(samplePagedata, 0, samplePagedata.Length);

            // List all blobs in this container. Because a container can contain a large number of blobs the results 
            // are returned in segments (pages) with a maximum of 5000 blobs per segment. You can define a smaller size
            // using the maxResults parameter on ListBlobsSegmentedAsync.
            Console.WriteLine("3. List Blobs in Container");
            BlobContinuationToken token = null;
            do
            {
                BlobResultSegment resultSegment = await container.ListBlobsSegmentedAsync(token);
                token = resultSegment.ContinuationToken;
                foreach (IListBlobItem blob in resultSegment.Results)
                {
                    // Blob type will be CloudBlockBlob, CloudPageBlob or CloudBlobDirectory
                    Console.WriteLine("{0} (type: {1}", blob.Uri, blob.GetType());
                }
            } while (token != null);
            
            // Read from a page blob
            //Console.WriteLine("4. Read from a Page Blob");
            int bytesRead = await pageBlob.DownloadRangeToByteArrayAsync(samplePagedata, 0, 0, samplePagedata.Count());

            // Clean up after the demo 
            Console.WriteLine("5. Delete page Blob");
            await pageBlob.DeleteAsync();

            // When you delete a container it could take several seconds before you can recreate a container with the same
            // name - hence to enable you to run the demo in quick succession the container is not deleted. If you want 
            // to delete the container uncomment the line of code below. 
            //Console.WriteLine("6. Delete Container");
            //await container.DeleteAsync();
        }


        ///<summary>
        ///
        /// Setting and Retrive Metadata
        /// 
        /// 
        /// </summary>
        public static async Task AddContainerMetadataAsync(CloudBlobContainer container)
        {
            try
            {
                //Adding some metadata to the container
                container.Metadata.Add("docType", "textDocumnets");
                container.Metadata["category"] = "guidance";

                //set the container's metadata
                await container.SetMetadataAsync();
            }
            catch (StorageException e)
            {
                

            }

        }


        ///<summary>
        ///To Retreive the metadata
        /// </summary>
        public static async Task ReadContainerMetadataAsync(CloudBlobContainer container)
        {
            try {

                await container.FetchAttributesAsync();

                Console.WriteLine("Container Metadata");

                foreach(var metaData in container.Metadata)
                {
                    Console.WriteLine("key:  " + metaData.Key);
                    Console.WriteLine("value:  " + metaData.Value);

                }


            }
            catch(StorageException e)
            {


            }


        }


        ///<summary>
        ///All the container properties, general properties likes storage uri, last modified
        /// </summary>
        private static async Task ReadContainerPropertiesAsync(CloudBlobContainer container)
        {
            try
            {
                // Fetch some container properties and write out their values.
                await container.FetchAttributesAsync();
                Console.WriteLine("Properties for container {0}", container.StorageUri.PrimaryUri);
                Console.WriteLine("Last modified time in UTC: {0}", container.Properties.LastModified);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }






        }
    }

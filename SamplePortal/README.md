HotDocs Open SDK Sample Portal
==============================

Table of Contents
-----------------

1. [About Sample Portal](#about-sample-portal)
2. [System Requirements](#system-requirements)
3. [Installing Sample Portal](#installing-sample-portal)
4. [Files Used with Sample Portal for HotDocs Server](#files-used-with-sample-portal-for-hotdocs-server)
5. [Using Sample Portal](#using-sample-portal)

### About Sample Portal

Sample Portal is a reference implementation of a host application that uses the HotDocs Open SDK. By using the open SDK, it can easily switch from utilizing a local instance of HotDocs Server to using either the HotDocs Server or Cloud Services web service, or vice versa as requirements change. It is not intended as a production-grade host application, but rather as an example of how the open SDK can be used when writing a host application.

Sample Portal includes the following three features:

* **Template Management**: Templates may be uploaded to the server, displayed in a template list, and used to assemble documents on the server.
* **Answer Management**: Answer files may be uploaded to the server and used with templates to assemble documents on the server. They can also be created and saved either on the server or on the local computer.
* **Document Creation**: At the end of assembly, documents may be downloaded to the local computer, but they cannot be saved on the server.

Sample Portal does not contain any user authentication or user management, and it is not to be used in any production-type environment.

### System Requirements

The hardware requirements for your system vary depending upon the type and complexity of the templates assembled. HotDocs Server performance is primarily bound by disk I/O speed, which means you should ensure that the disk system is as fast as possible. Also, additional RAM improves disk I/O speed by allowing the operating system to cache more files in system memory. The basic recommended hardware configuration is as follows:

* 2 or more processors (2 GHz or faster) (Single processor machines can work well for development and lower load production environments.)
* 1GB RAM (2 GB, recommended)
* SCSI or comparable storage subsystem

Your system must also meet the following software requirements before you can install Sample Portal:

* Microsoft Windows Server 2003 or 2008
* Microsoft .NET Framework 4.0
* Microsoft Internet Information Services (IIS) 5.1 or greater

If you are targeting a local HotDocs Server, you must install HotDocs Server 11.1 or greater on the same machine as Sample Portal. If you are targeting the HotDocs Server Web Service, you must install HotDocs Server 11.1 or greater along with its matching web service on a server that can be accessed from the Sample Portal machine; you will also need to have a shared network drive both machines can access. If you are targeting HotDocs Cloud Services, you will need a subscriber ID and signing key.

In addition to the requirements listed above for the server, end users who assemble documents using HotDocs Server must have certain software installed on their computers to complete browser-based interviews or view assembled documents. The following software is required:

* A Web browser capable of displaying an interview. JavaScript interviews require a modern browser (Chrome, Firefox, Safari, or Microsoft Internet Explorer 8.0 or later), and Silverlight interviews require a browser in which the Silverlight 5 runtime engine is installed.
* Any word processor that can display, edit, or print an assembled text document, including WordPerfect® and Microsoft® Word.
* HotDocs® Filler (for assembled HFD and HPD form documents) and/or Adobe® Reader® (for assembled PDF-based form documents).

### Installing Sample Portal

TODO: Extract the zip file and run the powershell script.

After completing the installation, verify that both HotDocs Server and your Web site are running. To begin using the Sample Portal, open Windows Explorer and enter the URL (e.g., http://www.domain.com/SamplePortal) in the Address box. (The last part of the URL, SamplePortal, will always be the same, but you must replace www.domain.com with your own domain or computer name.) When the page appears, you will see a list of templates ready for assembly.

### Files Used with Sample Portal for HotDocs Server

You should become familiar with the different files used by HotDocs Sample Portal, including DLL, JavaScript, and other ancillary files. The following table lists the important files, grouped by folder, which are installed with Sample Portal:

#### Sample Portal
This is the main program files folder (e.g., C:\Program Files\HotDocs Server\Sample Portal). It contains the the readme file and a shortcut to the Sample Portal home page.

#### Sample Portal\Files
This folder contains three subfolders:
* Answers: This folder contains the answer files that are created as users assemble documents and save their answers on the server. Also, the index.xml file in this folder contains the list of answer files that the Sample Portal displays on the Manage Answers page.
* Documents: This folder contains the documents that are assembled using the Sample Portal. Because the Sample Portal does not provide an interface for managing these documents, you should periodically delete the contents of this folder to free up disk space.
* Templates: As you upload templates to your server for use with the Sample Portal, they are copied to the Templates folder. The index.xml file in this folder contains the list of templates that users may assemble using the Sample Portal.
The user account under which the Web application runs (e.g., IUSR) must have read and write privileges to this folder in order to read and write answer files, documents, and templates in this folder.

#### Sample Portal\SamplePortal
The SamplePortal virtual directory created in IIS during the Sample Portal installation points to this folder. It contains the Sample Portal application and C# source code. The following folders and files are of particular interest:
* App_Code: This folder contains C# source code for the Sample Portal application.
* bin: This folder contains compiled binaries used by the Sample Portal application.
* images: This folder contains images that are displayed by the user interface.
* scripts: This folder contains some JavaScript files that are specific to Sample Portal.
* Default.aspx: This is the "home page" for Sample Portal.
* web.config: This configuration file is customized at the time of installation to contain the URLs and file paths to various HotDocs Server files required by the Sample Portal. Other application settings, such as the interview format and session time-out period, are specified in this file.

### Using Sample Portal

To begin using Sample Portal, you must "log on" to the Sample Portal Web site. (The term "log on" is used loosely since Sample Portal does not actually require a user name or password.) After logging on, Sample Portal provides an interface for you to assemble documents by merging a set of answers with a HotDocs template stored on the server. When you assemble a document, Sample Portal displays a set of dialogs (the "interview") where you enter answers required by the selected template. When you finish entering your answers in the interview, you can download and save the assembled document on your local computer.

Sample Portal also allows you to manage the set of templates available for assembly (a few sample templates from the HotDocs Demonstration Templates library are included, but you can also upload your own templates), and you can also manage answer files that are created when assembling documents.

#### To log on to Sample Portal
1. Open an Internet Explorer browser window.
2. Enter the Sample Portal URL in the Address bar. This URL looks like the following example, where www.domain.com is your own domain or computer name: http://www.domain.com/SamplePortal
When the Web site loads, you will see the Home page with a list of templates ready for assembly.

#### To assemble a document
1. At the **Home** (**Select a Template**) page, click the title of a template. The **Answer Set** page appears.
2. Choose a new answer file, an existing answer file on your PC, or an answer file stored on the server, then click **Continue**. The **Interview** page appears.
3. Answer questions on each dialog of the interview, then click **Finish** when all questions are answered. The Disposition page appears.
4. Do one or more of the following tasks:
    * If you did not answer all of the questions, click **Return to Interview** to finish answering required questions.
    * Enter a Title and Description for the answer file, then click **Save Answers** to save the answer file on the server.
    * Click **Download** to save a copy of the finished document(s) to your PC.

#### To manage templates
1. At the **Home** page, click **Manage Templates**. The **Manage Templates** page appears.
2. Click **Edit** next to a template to change its title or description.
3. Click **Delete** next to a template to remove it from the template list.
Note: Deleting a template in this manner does not delete the actual template files from the server. If you wish to delete the file, you must log on to the server and delete the files manually from the Sample Portal Templates folder (e.g., C:\Program Files\HotDocs Server\Sample Portal\Files\Templates).
Note: To upload additional templates to Sample Portal, follow the steps explained in the HotDocs Developer help file under the topic "Upload Templates to a Web Server." The URL for uploading templates is shown at the bottom of the Manage Templates page (e.g., http://www.domain.com/SamplePortal/upload.aspx).

#### To manage answer files
1. At the **Home** page, click **Manage Answers**. The **Manage Answer Files** page appears.
2. Click **Edit** next to an answer file to change its title or description.
3. Click **Delete** next to an answer file to remove it from the answer file list.
Note: You can only add a new answer file to this list by saving answers following a document assembly.

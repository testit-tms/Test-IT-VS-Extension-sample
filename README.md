# TestIT-VS-Extension-sample

## TestIT-VS-Extension-sample - Extension for Visual Studio using [Test IT](https://testit.software) system.
This is an extension for Visual Studio, which allows you to use the IDE resources to create autotests on the TestIT system.
You can get started with the extension by assembling it in your Visual Studio.

## Instruction for use
1.  Install Visual Studio SDK. [How-to](https://docs.microsoft.com/en-us/visualstudio/extensibility/installing-the-visual-studio-sdk?view=vs-2019).
2. Build this solution.
3. Find the `VSIXProject3.vsix` file in Debug/Release or another output catalog and install it.
4. Launch Visual Studio and create a new solution. To view a working example, add `UnitTestProject` project from this repository to your solution. Then assemble the solution.
5. Find TestIT on the toolbar, open it and go to settings. Fill in the fields. After that save the settings.
    - `URL`: URL to TestIT system, you can use `https://demo.testit.software/`;
    - `Secret key`: `$Secret key` of your account on the TestIT portal;
    - `Project name in Test IT`: name of your project on the TestIT portal;
    - `Test project`: your project containing tests in opened solution;
    - `Project dll`: assembly file (* .dll) of your test project;
    - `Repository link`: link to your repository.
6. Click `TestIT` - `Run`. The process of creating autotests in the Test IT system can be observed in the output window. 

### Developers
Visual Studio Extension: [Mihail Pirogovsky](https://github.com/developman2013)
   
Linker Library: [Savva Kozlovskiy](https://github.com/ltkirin), [Oleg Asmolovsky](https://github.com/Nicton)
   
Test Project: [Oleg Asmolovsky](https://github.com/Nicton)

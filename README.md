
# NIK SIK


## TODO

- [ ] add dash mechanic
- [ ] add the first level

## Installation

Navigate to where you want to install the project

Clone the project repo
```
git clone https://github.com/WAXDOG-ONLINE/NIK-SIK.git
```
Install [Unity hub](https://unity.com/download). Unity hub is the unity version manager

Try to add the cloned repo as a unity project in the unity hub

Install the suggested unity version for the unity project

## Formating

### Vscode Formatting

vscode should be configured to automatically format on save when this project is opened as the workspace. This is configured in this projects `.vscode/settings.json` file. 

If it is not working you can go the the vscode settings and search "format on save" and it should come up. 

If the formatting is not working on save you may need ot install some sort of formatter to take advantage of the projects `.editorconfig` file.

You can get a formatter as a vscode extensions such as [prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode). You can then go to your vscode settings and enable "Prettier: Use Editor Config" 






#### DOTNET formatting

you can also format the whole project at once, and not just a single file using something like the format command line tool from dotnet.

- **.NET SDK**: [Install the .NET SDK](https://dotnet.microsoft.com/download) if you haven't already.

#### Install `dotnet-format`


Install `dotnet-format` as a global tool via the command line:

```bash
dotnet tool install -g dotnet-format
```

> **Note:** To update an existing installation, run:
>
> ```bash
> dotnet tool update -g dotnet-format
> ```


> **Note:** If you run into errors about the current project having conflicts, try navigating to the root directory and trying the installation again
>
> ```bash
> cd ~
> ```

### Format the Project

1. **Navigate to Project Root:**

   Open your terminal and change to the root directory of your Unity project where the `.editorconfig` file is located:

   ```bash
   cd path/to/NIK-SIK
   ```

2. **Run `dotnet-format`:**

   Execute the following command to format all C# files based on the `.editorconfig` rules:

   ```bash
   dotnet format
   ```

### Verify Formatting (Optional)

To check if your code adheres to the formatting rules without making changes, use:

```bash
dotnet format --check
```

- [dotnet-format Documentation](https://github.com/dotnet/format)
- [EditorConfig Documentation](https://editorconfig.org/)




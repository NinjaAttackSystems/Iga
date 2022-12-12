using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AndroidX.ConstraintLayout.Motion.Widget;
using GerberVS;

namespace GbrToGcode.Services
{
    public class GerberService
    {

        private LibGerberVS gerberLib = null;
        private GerberProject project = null;
        private GerberRenderInformation renderInfo = null;
        private SelectionInformation selectionInfo = null; // List containing the currenly selected objects (nets).

        bool hasProject = false;
        string clFileName = String.Empty;
        string formName = String.Empty;
        StringBuilder logText;
        private float userScale;
        private float userTranslateX;
        private float userTranslateY;
        private float userRotation;
        private bool userMirrorX;
        private bool userMirrorY;
        private bool userInverted;

        private bool selectTool = true;

        private void OpenLayers(string[] fileList)
        {
            int index = -1;
            foreach (string file in fileList)
            {
                // Don't open gerber project files here.
                if (Path.GetExtension(file).ToLower() == ".gpf")
                    continue;

                if (OpenLayer(file))
                {
                    // if (fileListBox.SelectedIndex == -1)
                    // fileListBox.SelectedIndex = 0;

                    index = project.FileCount - 1;
                    // fileListBox.AddItem(project.FileInfo[index].IsVisible, project.FileInfo[index].Color,
                        // project.FileInfo[index].FileName);
                    // LayerNameToolStripStatusLabel.Text = project.FileInfo[fileListBox.SelectedIndex].FileName;
                    // logText.AppendLine("Open Layer: " + project.FileInfo[index].FullPathName);

                    // if (project.FileInfo[index].Image.FileType == GerberFileType.RS274X)
                    // {
                        // LogMessages(project.FileInfo[index].Image.GerberStats.ErrorList);
                    // }

                    // else if (project.FileInfo[index].Image.FileType == GerberFileType.Drill)
                    // {
                        // LogMessages(project.FileInfo[index].Image.DrillStats.ErrorList);
                    // }

                    // UpdateLog();
                }

            }

            TranslateImage();
            pcbImagePanel.Invalidate();
            UpdateMenus();
            if (project.FileCount > 0)
                hasProject = true;

            if (project.ProjectName == string.Empty)
                Text = formName + " [Untitled Project]";
        }

        // Open layer file and if sucessful, add it to file list box.
        private bool OpenLayer(string fileName)
        {
            try
            {
                gerberLib.OpenLayerFromFileName(project, fileName);
                return true;
            }

            catch (GerberDllException ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += ex.InnerException.Message;

                Console.WriteLine(errorMessage);
                return false;
            }
        }
        
    }
}
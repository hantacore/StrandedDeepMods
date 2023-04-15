using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StrandedDeepMapper
{
    public partial class RandomizerInstructions : Form
    {
        public RandomizerInstructions()
        {
            InitializeComponent();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("STEP 1");
            sb.AppendLine("Generate a new world inside Stranded Deep");
            sb.AppendLine("Do not start a new game");
            sb.AppendLine("Exit game");
            sb.AppendLine();
            sb.AppendLine("STEP 2");
            sb.AppendLine("Launch this tool");
            sb.AppendLine("Locate your Data folder (if not detected automatically)");
            sb.AppendLine("Locate your Stranded Deep folder (necessary for missions)");
            sb.AppendLine();
            sb.AppendLine("STEP 3 (optional)");
            sb.AppendLine("I would recommend to backup your current world.");
            sb.AppendLine("This is done by clicking on the \"Backup world\" button");
            sb.AppendLine();
            sb.AppendLine("STEP 4");
            sb.AppendLine("Choose your world options : world size, mission usage, maps to use, where you want to start");
            sb.AppendLine("Check the islands you would like to have in your world (the tool will pick some, but not necessarily every single one)");
            sb.AppendLine();
            sb.AppendLine("STEP 5");
            sb.AppendLine("Click on GENERATE WORLD");
            sb.AppendLine();
            sb.AppendLine("STEP 6");
            sb.AppendLine("Launch the game");
            sb.AppendLine("Choose \"New game\"");
            sb.AppendLine("Choose \"World : Existing\"");
            sb.AppendLine("Enjoy !");
            sb.AppendLine();
            sb.AppendLine("IMPORTANT WARNING");
            sb.AppendLine("I did not test it with experimental version, so i cannot guarantee it will work, even if it should, so, you've been warned now");
            sb.AppendLine();
            sb.AppendLine("BONUS FOR MAP-MAKERS");
            sb.AppendLine("By right clicking on an island, you can open its directory (easier to look that random guids ;))");

            rtbInstructions.Text = sb.ToString();
        }
    }
}

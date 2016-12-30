namespace WPFBingMapPOC
{
    partial class MapForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapForm));
            this.elementHost = new System.Windows.Forms.Integration.ElementHost();
            this.bingMapsControl = new UserControls.MapControl();
            this.SuspendLayout();
            // 
            // elementHost
            // 
            this.elementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.elementHost.Location = new System.Drawing.Point(0, 0);
            this.elementHost.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.elementHost.Name = "elementHost";
            this.elementHost.Size = new System.Drawing.Size(503, 341);
            this.elementHost.TabIndex = 2;
            this.elementHost.Text = "elementHost";
            this.elementHost.Child = this.bingMapsControl;
            // 
            // MapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 341);
            this.Controls.Add(this.elementHost);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "MapForm";
            this.Text = "Bing Maps";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Integration.ElementHost elementHost;
        private UserControls.MapControl bingMapsControl;
    }
}


namespace BingMapsPOC
{
    partial class WebMapForm
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
            this.layer1 = new DevExpress.XtraMap.ImageLayer();
            this.bingMapDataProvider1 = new DevExpress.XtraMap.BingMapDataProvider();
            this.vectorItemsLayer1 = new DevExpress.XtraMap.VectorItemsLayer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // bingMapDataProvider1
            // 
            this.bingMapDataProvider1.BingKey = "AsJ2NC7HdNc4k8T03GPuBT5IwzYUJATkSTWfaog7uEJ0lsx_XLS3st1ZStjitTm2";
            this.bingMapDataProvider1.Kind = DevExpress.XtraMap.BingMapKind.Road;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(635, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(120, 525);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // WebMapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 525);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "WebMapForm";
            this.Text = "Bing Maps";
            this.ResumeLayout(false);

        }

        #endregion
        private DevExpress.XtraMap.ImageLayer layer1;
        private DevExpress.XtraMap.BingMapDataProvider bingMapDataProvider1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraMap.VectorItemsLayer vectorItemsLayer1;
    }
}


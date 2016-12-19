namespace DevExpressPOC
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
            this.mapControl1 = new DevExpress.XtraMap.MapControl();
            this.layer1 = new DevExpress.XtraMap.ImageLayer();
            this.bingMapDataProvider1 = new DevExpress.XtraMap.BingMapDataProvider();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.vectorItemsLayer1 = new DevExpress.XtraMap.VectorItemsLayer();
            ((System.ComponentModel.ISupportInitialize)(this.mapControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // mapControl1
            // 
            this.mapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mapControl1.CenterPoint = new DevExpress.XtraMap.GeoPoint(53D, -5D);
            this.mapControl1.Layers.Add(this.layer1);
            this.mapControl1.Layers.Add(this.vectorItemsLayer1);
            this.mapControl1.Location = new System.Drawing.Point(0, 0);
            this.mapControl1.Name = "mapControl1";
            this.mapControl1.Size = new System.Drawing.Size(633, 525);
            this.mapControl1.TabIndex = 0;
            this.mapControl1.ZoomLevel = 5D;
            // 
            // layer1
            // 
            this.layer1.DataProvider = this.bingMapDataProvider1;
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
            // MapForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 525);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.mapControl1);
            this.Name = "MapForm";
            this.Text = "Bing Maps";
            ((System.ComponentModel.ISupportInitialize)(this.mapControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraMap.MapControl mapControl1;
        private DevExpress.XtraMap.ImageLayer layer1;
        private DevExpress.XtraMap.BingMapDataProvider bingMapDataProvider1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DevExpress.XtraMap.VectorItemsLayer vectorItemsLayer1;
    }
}


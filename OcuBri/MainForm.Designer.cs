namespace OcuBri
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblOculusRiftConnect = new System.Windows.Forms.Label();
            this.lblMainWSConnect = new System.Windows.Forms.Label();
            this.lblOculusWSConnect = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(12, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 37);
            this.label1.TabIndex = 0;
            this.label1.Text = "Oculus Rift :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(13, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 37);
            this.label2.TabIndex = 1;
            this.label2.Text = "Main WS :";
            // 
            // lblOculusRiftConnect
            // 
            this.lblOculusRiftConnect.AutoSize = true;
            this.lblOculusRiftConnect.Font = new System.Drawing.Font("MS UI Gothic", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblOculusRiftConnect.Location = new System.Drawing.Point(245, 27);
            this.lblOculusRiftConnect.Name = "lblOculusRiftConnect";
            this.lblOculusRiftConnect.Size = new System.Drawing.Size(242, 37);
            this.lblOculusRiftConnect.TabIndex = 2;
            this.lblOculusRiftConnect.Text = "None Connect";
            // 
            // lblMainWSConnect
            // 
            this.lblMainWSConnect.AutoSize = true;
            this.lblMainWSConnect.Font = new System.Drawing.Font("MS UI Gothic", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMainWSConnect.Location = new System.Drawing.Point(245, 90);
            this.lblMainWSConnect.Name = "lblMainWSConnect";
            this.lblMainWSConnect.Size = new System.Drawing.Size(242, 37);
            this.lblMainWSConnect.TabIndex = 3;
            this.lblMainWSConnect.Text = "None Connect";
            // 
            // lblOculusWSConnect
            // 
            this.lblOculusWSConnect.AutoSize = true;
            this.lblOculusWSConnect.Font = new System.Drawing.Font("MS UI Gothic", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblOculusWSConnect.Location = new System.Drawing.Point(245, 150);
            this.lblOculusWSConnect.Name = "lblOculusWSConnect";
            this.lblOculusWSConnect.Size = new System.Drawing.Size(242, 37);
            this.lblOculusWSConnect.TabIndex = 5;
            this.lblOculusWSConnect.Text = "None Connect";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(13, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(204, 37);
            this.label4.TabIndex = 4;
            this.label4.Text = "Oculus WS :";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 209);
            this.Controls.Add(this.lblOculusWSConnect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblMainWSConnect);
            this.Controls.Add(this.lblOculusRiftConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "OcuBri";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        internal System.Windows.Forms.Label lblOculusRiftConnect;
        internal System.Windows.Forms.Label lblMainWSConnect;
        internal System.Windows.Forms.Label lblOculusWSConnect;
        private System.Windows.Forms.Label label4;
    }
}


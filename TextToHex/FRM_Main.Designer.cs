namespace TextToHex
{
    partial class FRM_Menu
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FRM_Menu));
            this.TXT_string = new System.Windows.Forms.TextBox();
            this.TXT_Hexa = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BT_convertisseursUno = new System.Windows.Forms.Button();
            this.GBOX_txthex = new System.Windows.Forms.GroupBox();
            this.GBOX_hexbin = new System.Windows.Forms.GroupBox();
            this.TXT_nombres = new System.Windows.Forms.TextBox();
            this.TXT_bin = new System.Windows.Forms.TextBox();
            this.TXT_hex = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BT_convertisseurs = new System.Windows.Forms.Button();
            this.BT_clear = new System.Windows.Forms.Button();
            this.GBOX_txthex.SuspendLayout();
            this.GBOX_hexbin.SuspendLayout();
            this.SuspendLayout();
            // 
            // TXT_string
            // 
            this.TXT_string.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_string.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TXT_string.Location = new System.Drawing.Point(116, 26);
            this.TXT_string.Name = "TXT_string";
            this.TXT_string.Size = new System.Drawing.Size(656, 31);
            this.TXT_string.TabIndex = 0;
            // 
            // TXT_Hexa
            // 
            this.TXT_Hexa.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_Hexa.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TXT_Hexa.Location = new System.Drawing.Point(116, 75);
            this.TXT_Hexa.Name = "TXT_Hexa";
            this.TXT_Hexa.Size = new System.Drawing.Size(656, 31);
            this.TXT_Hexa.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(34, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 19);
            this.label1.TabIndex = 2;
            this.label1.Text = "Texte : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(37, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 19);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hexa : ";
            // 
            // BT_convertisseursUno
            // 
            this.BT_convertisseursUno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_convertisseursUno.ForeColor = System.Drawing.Color.Black;
            this.BT_convertisseursUno.Location = new System.Drawing.Point(622, 121);
            this.BT_convertisseursUno.Name = "BT_convertisseursUno";
            this.BT_convertisseursUno.Size = new System.Drawing.Size(150, 38);
            this.BT_convertisseursUno.TabIndex = 4;
            this.BT_convertisseursUno.Text = "Convertir";
            this.BT_convertisseursUno.UseVisualStyleBackColor = true;
            this.BT_convertisseursUno.Click += new System.EventHandler(this.BT_convertisseursUno_Click);
            // 
            // GBOX_txthex
            // 
            this.GBOX_txthex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GBOX_txthex.BackColor = System.Drawing.Color.Transparent;
            this.GBOX_txthex.Controls.Add(this.BT_convertisseursUno);
            this.GBOX_txthex.Controls.Add(this.label2);
            this.GBOX_txthex.Controls.Add(this.TXT_string);
            this.GBOX_txthex.Controls.Add(this.label1);
            this.GBOX_txthex.Controls.Add(this.TXT_Hexa);
            this.GBOX_txthex.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GBOX_txthex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(186)))), ((int)(((byte)(0)))));
            this.GBOX_txthex.Location = new System.Drawing.Point(12, 12);
            this.GBOX_txthex.Name = "GBOX_txthex";
            this.GBOX_txthex.Size = new System.Drawing.Size(778, 165);
            this.GBOX_txthex.TabIndex = 5;
            this.GBOX_txthex.TabStop = false;
            this.GBOX_txthex.Text = "Texte vers hexa";
            // 
            // GBOX_hexbin
            // 
            this.GBOX_hexbin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GBOX_hexbin.BackColor = System.Drawing.Color.Transparent;
            this.GBOX_hexbin.Controls.Add(this.BT_clear);
            this.GBOX_hexbin.Controls.Add(this.BT_convertisseurs);
            this.GBOX_hexbin.Controls.Add(this.label5);
            this.GBOX_hexbin.Controls.Add(this.label4);
            this.GBOX_hexbin.Controls.Add(this.label3);
            this.GBOX_hexbin.Controls.Add(this.TXT_hex);
            this.GBOX_hexbin.Controls.Add(this.TXT_bin);
            this.GBOX_hexbin.Controls.Add(this.TXT_nombres);
            this.GBOX_hexbin.Font = new System.Drawing.Font("Trebuchet MS", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GBOX_hexbin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(186)))), ((int)(((byte)(0)))));
            this.GBOX_hexbin.Location = new System.Drawing.Point(12, 195);
            this.GBOX_hexbin.Name = "GBOX_hexbin";
            this.GBOX_hexbin.Size = new System.Drawing.Size(778, 246);
            this.GBOX_hexbin.TabIndex = 6;
            this.GBOX_hexbin.TabStop = false;
            this.GBOX_hexbin.Text = "Nombres vers hexa et bin";
            // 
            // TXT_nombres
            // 
            this.TXT_nombres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_nombres.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TXT_nombres.Location = new System.Drawing.Point(516, 38);
            this.TXT_nombres.Name = "TXT_nombres";
            this.TXT_nombres.Size = new System.Drawing.Size(256, 31);
            this.TXT_nombres.TabIndex = 0;
            // 
            // TXT_bin
            // 
            this.TXT_bin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_bin.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TXT_bin.Location = new System.Drawing.Point(269, 137);
            this.TXT_bin.Name = "TXT_bin";
            this.TXT_bin.Size = new System.Drawing.Size(503, 31);
            this.TXT_bin.TabIndex = 1;
            // 
            // TXT_hex
            // 
            this.TXT_hex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_hex.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TXT_hex.Location = new System.Drawing.Point(269, 193);
            this.TXT_hex.Name = "TXT_hex";
            this.TXT_hex.Size = new System.Drawing.Size(503, 31);
            this.TXT_hex.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(404, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "Nombres : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(171, 143);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 19);
            this.label4.TabIndex = 4;
            this.label4.Text = "Binaire : ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(187, 199);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 19);
            this.label5.TabIndex = 5;
            this.label5.Text = "Hexa : ";
            // 
            // BT_convertisseurs
            // 
            this.BT_convertisseurs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_convertisseurs.ForeColor = System.Drawing.Color.Black;
            this.BT_convertisseurs.Location = new System.Drawing.Point(418, 75);
            this.BT_convertisseurs.Name = "BT_convertisseurs";
            this.BT_convertisseurs.Size = new System.Drawing.Size(174, 33);
            this.BT_convertisseurs.TabIndex = 6;
            this.BT_convertisseurs.Text = "Convertir";
            this.BT_convertisseurs.UseVisualStyleBackColor = true;
            this.BT_convertisseurs.Click += new System.EventHandler(this.BT_convertisseurs_Click);
            // 
            // BT_clear
            // 
            this.BT_clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BT_clear.ForeColor = System.Drawing.Color.Black;
            this.BT_clear.Location = new System.Drawing.Point(598, 75);
            this.BT_clear.Name = "BT_clear";
            this.BT_clear.Size = new System.Drawing.Size(174, 33);
            this.BT_clear.TabIndex = 7;
            this.BT_clear.Text = "Vider TextBox";
            this.BT_clear.UseVisualStyleBackColor = true;
            this.BT_clear.Click += new System.EventHandler(this.BT_clear_Click);
            // 
            // FRM_Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(36)))), ((int)(((byte)(36)))));
            this.ClientSize = new System.Drawing.Size(802, 453);
            this.Controls.Add(this.GBOX_hexbin);
            this.Controls.Add(this.GBOX_txthex);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FRM_Menu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Convertisseur";
            this.GBOX_txthex.ResumeLayout(false);
            this.GBOX_txthex.PerformLayout();
            this.GBOX_hexbin.ResumeLayout(false);
            this.GBOX_hexbin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_string;
        private System.Windows.Forms.TextBox TXT_Hexa;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BT_convertisseursUno;
        private System.Windows.Forms.GroupBox GBOX_txthex;
        private System.Windows.Forms.GroupBox GBOX_hexbin;
        private System.Windows.Forms.TextBox TXT_nombres;
        private System.Windows.Forms.TextBox TXT_hex;
        private System.Windows.Forms.TextBox TXT_bin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button BT_convertisseurs;
        private System.Windows.Forms.Button BT_clear;
    }
}


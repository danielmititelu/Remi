using System;
using System.IO;
using System.Windows.Forms;
namespace WindowsFormsApplication1 {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components=null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose (bool disposing) {
            if (disposing) {
               // using (writer=new StreamWriter (serverStream)) {
                    writer=new StreamWriter (serverStream);
                    writer.WriteLine ("EXT:Am iesit");
                    writer.Flush ();
                    //writer.Close ();
                    //reader.Close ();
                    //tcpClient.Close ();
                    //serverStream.Close ();
                //}
            }

            if (disposing&&(components!=null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
            //this.Dispose (disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent () {
            this.textbox=new System.Windows.Forms.TextBox ();
            this.send=new System.Windows.Forms.TextBox ();
            this.textBox1=new System.Windows.Forms.TextBox ();
            this.textBox2=new System.Windows.Forms.TextBox ();
            this.textBox3=new System.Windows.Forms.TextBox ();
            this.label2=new System.Windows.Forms.Label ();
            this.label3=new System.Windows.Forms.Label ();
            this.label4=new System.Windows.Forms.Label ();
            this.button2=new System.Windows.Forms.Button ();
            this.error=new System.Windows.Forms.Label ();
            this.label1=new System.Windows.Forms.Label ();
            this.listBox1=new System.Windows.Forms.ListBox ();
            this.SuspendLayout ();
            // 
            // textbox
            // 
            this.textbox.Location=new System.Drawing.Point (132, 114);
            this.textbox.Multiline=true;
            this.textbox.Name="textbox";
            this.textbox.ReadOnly=true;
            this.textbox.Size=new System.Drawing.Size (372, 225);
            this.textbox.TabIndex=3;
            // 
            // send
            // 
            this.send.Location=new System.Drawing.Point (132, 355);
            this.send.Name="send";
            this.send.Size=new System.Drawing.Size (372, 22);
            this.send.TabIndex=4;
            this.send.KeyDown+=new System.Windows.Forms.KeyEventHandler (this.send_KeyDown);
            // 
            // textBox1
            // 
            this.textBox1.Location=new System.Drawing.Point (307, 10);
            this.textBox1.Name="textBox1";
            this.textBox1.Size=new System.Drawing.Size (100, 22);
            this.textBox1.TabIndex=5;
            // 
            // textBox2
            // 
            this.textBox2.Location=new System.Drawing.Point (307, 47);
            this.textBox2.Name="textBox2";
            this.textBox2.Size=new System.Drawing.Size (100, 22);
            this.textBox2.TabIndex=6;
            // 
            // textBox3
            // 
            this.textBox3.Location=new System.Drawing.Point (307, 84);
            this.textBox3.Name="textBox3";
            this.textBox3.Size=new System.Drawing.Size (100, 22);
            this.textBox3.TabIndex=7;
            // 
            // label2
            // 
            this.label2.AutoSize=true;
            this.label2.Location=new System.Drawing.Point (283, 15);
            this.label2.Name="label2";
            this.label2.Size=new System.Drawing.Size (17, 17);
            this.label2.TabIndex=8;
            this.label2.Text="A";
            // 
            // label3
            // 
            this.label3.AutoSize=true;
            this.label3.Location=new System.Drawing.Point (283, 50);
            this.label3.Name="label3";
            this.label3.Size=new System.Drawing.Size (17, 17);
            this.label3.TabIndex=9;
            this.label3.Text="B";
            // 
            // label4
            // 
            this.label4.AutoSize=true;
            this.label4.Location=new System.Drawing.Point (283, 84);
            this.label4.Name="label4";
            this.label4.Size=new System.Drawing.Size (17, 17);
            this.label4.TabIndex=10;
            this.label4.Text="C";
            // 
            // button2
            // 
            this.button2.Location=new System.Drawing.Point (429, 44);
            this.button2.Name="button2";
            this.button2.Size=new System.Drawing.Size (75, 23);
            this.button2.TabIndex=11;
            this.button2.Text="Testeaza";
            this.button2.UseVisualStyleBackColor=true;
            this.button2.Click+=new System.EventHandler (this.button2_Click);
            // 
            // error
            // 
            this.error.AutoSize=true;
            this.error.Location=new System.Drawing.Point (229, 394);
            this.error.Name="error";
            this.error.Size=new System.Drawing.Size (0, 17);
            this.error.TabIndex=12;
            // 
            // label1
            // 
            this.label1.AutoSize=true;
            this.label1.Location=new System.Drawing.Point (36, 24);
            this.label1.Name="label1";
            this.label1.Size=new System.Drawing.Size (49, 17);
            this.label1.TabIndex=14;
            this.label1.Text="Users:";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled=true;
            this.listBox1.ItemHeight=16;
            this.listBox1.Location=new System.Drawing.Point (12, 50);
            this.listBox1.Name="listBox1";
            this.listBox1.Size=new System.Drawing.Size (101, 324);
            this.listBox1.TabIndex=15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions=new System.Drawing.SizeF (8F, 16F);
            this.AutoScaleMode=System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize=new System.Drawing.Size (540, 420);
            this.Controls.Add (this.listBox1);
            this.Controls.Add (this.label1);
            this.Controls.Add (this.error);
            this.Controls.Add (this.button2);
            this.Controls.Add (this.label4);
            this.Controls.Add (this.label3);
            this.Controls.Add (this.label2);
            this.Controls.Add (this.textBox3);
            this.Controls.Add (this.textBox2);
            this.Controls.Add (this.textBox1);
            this.Controls.Add (this.send);
            this.Controls.Add (this.textbox);
            this.Name="Form1";
            this.Text="Form1";
            this.ResumeLayout (false);
            this.PerformLayout ();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox;
        private System.Windows.Forms.TextBox send;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label2;
        private Label label3;
        private Label label4;
        private Button button2;
        private Label error;
        private Label label1;
        private ListBox listBox1;
    }
}


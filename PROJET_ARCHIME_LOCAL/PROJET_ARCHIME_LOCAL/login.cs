using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data.OleDb;
using MySql.Data.MySqlClient;

namespace PROJET_ARCHIME_LOCAL
{
    public partial class connexion : Form
    {
        private const string CHEMIN_BASE_MDB = "C:\\archimed_bd.mdb";
        public connexion()
        {
            InitializeComponent();
        }

      

        private void connexion_Load(object sender, EventArgs e)
        {
            lbl_error.ForeColor = Color.Red;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txt_username.Text == "")
            {
                lbl_error.Text = "Definissez le nom utilisateur";
            }
            else
            {
                if(txt_password.Text == "")
                {
                    lbl_error.Text = "Entrez le mot de passe !";
                }
                else
                {
                        //continuer

                        MySqlConnection dbConn = new MySqlConnection("Persist Security Info=False;server=localhost;database=archimed_new;uid=root;password=root");
                        MySqlCommand cmd = dbConn.CreateCommand();
                        cmd.CommandText = "SELECT password from user WHERE username = @username";
                        cmd.Parameters.AddWithValue("@username", txt_username.Text);

                        try
                        {
                            dbConn.Open();
                        }
                        catch (Exception erro)
                        {
                            MessageBox.Show("Erro" + erro);
                            this.Close();
                        }

                        MySqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            if(reader.GetString(0) == txt_password.Text)
                            {
                                Form frm = new Acceuil_interface();
                                frm.Show();


                                //trouver moyen de le fermer 
                                this.Hide();
                            }
                            else
                            {
                            MessageBox.Show("Mauvais mot de passe!");
                        }
                        }
                        else
                        {
                            MessageBox.Show("Mauvais nom utilisateur!");
                        }
                    }
                   
                }
            }
                
            
        }
    
}

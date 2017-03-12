using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace PROJET_ARCHIME_LOCAL
{
    class Db_carte
    {
        
        public void copie_db_carte(string lecteur)
        {
            
            string fileName = "bd_archimede.sdf";
            string sourcePath = @""+ lecteur +":\\Database\\";
            string targetPath = Application.StartupPath + "\\Database_carte\\";

            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);
            // To copy a file to another location and 
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);
            MessageBox.Show("carte copiée");
        }
        public void synchro_add_patient()
        {
            string con_string = "Data Source = " + Application.StartupPath + "\\Database_carte\\bd_archimede.sdf";
            SqlCeConnection dbConn_carte = new SqlCeConnection(con_string);
            try
            {
                dbConn_carte.Open();
            }
            catch (Exception erro)
            {
                MessageBox.Show("Erro" + erro);
                //this.Close();
            }
            using (SqlCeCommand com = new SqlCeCommand("SELECT * FROM patient", dbConn_carte))
            {
                SqlCeDataReader reader_carte = com.ExecuteReader();
                while (reader_carte.Read())
                {
                    int num = reader_carte.GetInt32(0); // a remplacer, dépendant de ce que tu veux lire
                    
                    MessageBox.Show("Lidentifiant patient est : "+num);


                    /// insert dans la base de donnée sql
                    MySqlConnection dbConn = new MySqlConnection("Persist Security Info=False;server=localhost;database=archimed_new;uid=root;password=root");
                    MySqlCommand cmd = dbConn.CreateCommand();
                    cmd.CommandText = "INSERT INTO patient(id, matricule, nom, prenom, nomMarital, dateNaissance, lieuNaissance, trancheAge, telephone, telephoneUrgence, mail, domicile, ville, pays, antecedentPatient_id) VALUES(@id, @matricule, @nom, @prenom, @nomMarital, @dateNaissance, @lieuNaissance, @trancheAge, @telephone, @telephoneUrgence, @mail, @domicile, @ville, @pays, @antecedentPatient) ";
                    cmd.Parameters.AddWithValue("@id", null);
                    cmd.Parameters.AddWithValue("@matricule", reader_carte.GetString(1));
                    cmd.Parameters.AddWithValue("@nom", reader_carte.GetString(2));
                    cmd.Parameters.AddWithValue("@prenom", reader_carte.GetString(3));
                    cmd.Parameters.AddWithValue("@nomMarital", null);
                    cmd.Parameters.AddWithValue("@dateNaissance", reader_carte.GetString(5));
                    cmd.Parameters.AddWithValue("@lieuNaissance", reader_carte.GetString(6));
                    cmd.Parameters.AddWithValue("@trancheAge", reader_carte.GetString(7));
                    cmd.Parameters.AddWithValue("@telephone", reader_carte.GetString(8));
                    cmd.Parameters.AddWithValue("@telephoneUrgence", reader_carte.GetString(9));
                    cmd.Parameters.AddWithValue("@mail", reader_carte.GetString(10));
                    cmd.Parameters.AddWithValue("@domicile", reader_carte.GetString(11));
                    cmd.Parameters.AddWithValue("@ville", null);
                    cmd.Parameters.AddWithValue("@pays", null);
                    cmd.Parameters.AddWithValue("@antecedentPatient", 0);

                    dbConn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("patient ajouté");
                        Acceuil_interface classPrincipale = new Acceuil_interface();
                        classPrincipale.add_file(reader_carte.GetString(1));
                    }
                    catch (Exception erro)
                    {
                        MessageBox.Show("Erro" + erro);
                        //this.Close();
                    }
                   
                }
            }
            

            MessageBox.Show("patient en base table \"patient\"");
        }
    }
}

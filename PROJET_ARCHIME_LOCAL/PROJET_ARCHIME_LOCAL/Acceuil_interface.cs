using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROJET_ARCHIME_LOCAL
{
    public partial class Acceuil_interface : Form
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x2;
        Db_carte carte = new Db_carte();

        public Acceuil_interface()
        {
            InitializeComponent();
        }
        /// <summary>
		/// Surcharge de la fonction de Message Loop, WndProc
		/// </summary>
		/// <param name="m">Message reçu dans la boucle de messages</param>
		protected override void WndProc(ref Message m)
        {
            // le message est de type DEVICECHANGE, ce qui nous interesse
            if (m.Msg == WM_DEVICECHANGE)
            {
                // le "sous-message" dit que le device vient d'etre pluggé
                if (m.WParam.ToInt32() == DBT_DEVICEARRIVAL)
                {
                    // device plugged

                    // on créé une structure depuis un pointeur a l'aide du Marshalling
                    // cette structure est generique mais on peut "l'interroger" comme une structure DEV_BROADCAST_HDR
                    DEV_BROADCAST_HDR hdr = (DEV_BROADCAST_HDR)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_HDR));

                    // ok, le device pluggé est un volume (aussi appelé 'périphérique de stockage de masse')...
                    if (hdr.dbch_devicetype == DBT_DEVTYP_VOLUME)
                    {
                        // ... et donc on recréé une structure, a partir du même pointeur de structure "générique",
                        // une structure un poil plus spécifique
                        DEV_BROADCAST_VOLUME vol = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(m.LParam, typeof(DEV_BROADCAST_VOLUME));
                        // le champs dbcv_unitmask contient la ou les lettres de lecteur du ou des devices qui viennent d'etre pluggé
                        // MSDN nous dit que si le bit 0 est à 1 alors le lecteur est a:, si le bit 1 est à 1 alors le lecteur est b:
                        // et ainsi de suite
                        uint mask = vol.dbcv_unitmask;
                        // recupèration des lettres de lecteurs
                        char[] letters = MaskDepioteur(mask);

                        // mise à jour de l'IHM pour notifier nos petits yeux tout content :)
                        this.Text = string.Format("USB key plugged on drive {0}:", letters[0].ToString().ToUpper());
                        if (File.Exists(letters[0].ToString().ToUpper() + ":\\texto.txt"))
                        {
                            MessageBox.Show("c'est une clé archimed");
                            carte.copie_db_carte(letters[0].ToString().ToUpper());
                            carte.synchro_add_patient();
                        }
                        else
                        {
                            MessageBox.Show("ce n'est pas une clé archimed");
                        }
                    }
                }
                // le device vient d'etre retirer bourrinement ou proprement
                // (ce message intervient même quand on défait la clef softwarement mais qu'elle reste physiquement branché)
                else if (m.WParam.ToInt32() == DBT_DEVICEREMOVECOMPLETE)
                {
                    // device removed

                    // mise à jour de l'IHM
                    this.Text = "USB key unplugged";
                }
            }

            // laissons notre fenêtre faire tout de même son boulot
            base.WndProc(ref m);
        }

        // fonction d'extraction des lettres de lecteur
        public static char[] MaskDepioteur(uint mask)
        {
            int cnt = 0;
            uint temp_mask = mask;

            // on compte le nombre de bits à 1
            for (int i = 0; i < 32; i++)
            {
                if ((temp_mask & 1) == 1)
                    cnt++;
                temp_mask >>= 1;
                if (temp_mask == 0)
                    break;
            }

            // on instancie le bon nombre d'elements
            char[] result = new char[cnt];
            cnt = 0;
            // on refait mais ce coup ci on attribut
            for (int i = 0; i < 32; i++)
            {
                if ((mask & 1) == 1)
                    result[cnt++] = (char)('a' + i);
                mask >>= 1;
                if (mask == 0)
                    break;
            }

            return (result);
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void combo_specialiste_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void Acceuil_interface_Load(object sender, EventArgs e)
        {
                //regarder dans la base de donnée les patient dans la file d'attente
            MySqlConnection dbConn = new MySqlConnection("Persist Security Info=False;server=localhost;database=archimed_new;uid=root;password=root");
            MySqlCommand cmd = dbConn.CreateCommand();
            cmd.CommandText = "SELECT mat_patient from attente";

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
            var myArray2 = new string[100];
            int i = 0;
            while (reader.Read())
            {
                myArray2[i] = reader[0].ToString();
                
               // MessageBox.Show("" + myArray2[i]);
                add_file(myArray2[i]);
                i++;
            }
            reader.Close();


            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form login_form = new connexion();
            login_form.Show();
            this.Close();
        }

        public void add_file(string matric)
        {
            var matricule = new string[100];
            int c = 0;
            MySqlConnection dbConn = new MySqlConnection("Persist Security Info=False;server=localhost;database=archimed_new;uid=root;password=root");
            MySqlCommand read_patient = dbConn.CreateCommand();

           // MessageBox.Show("SELECT * FROM patient WHERE matricule='" + matric + "'");
            read_patient.CommandText = "SELECT * FROM patient WHERE matricule='" + matric + "'";

            dbConn.Open();


            MySqlDataReader readerP = read_patient.ExecuteReader();
            while (readerP.Read())
            {
                
                matricule[c] = readerP[1].ToString();

                var grp_patient = new GroupBox
                {
                    Width = 245,
                    Height = 100,
                    BackColor = Color.FromArgb(254, 254, 254),
                };
                var lbl_matricule = new Label
                {
                    Text = "Matricule : " + readerP[1].ToString(),
                    Location = new Point(100, 50),
                };
                var lbl_nom = new Label
                {
                    Text = "Nom : " + readerP[2].ToString(),
                    Location = new Point(100, 10)
                };
                var lbl_Pnom = new Label
                {
                    Text = "Prenom(s) : " + readerP[3].ToString(),
                    Location = new Point(100, 30)
                };
                var picture_patient = new PictureBox
                {
                    Width = 80,
                    Height = 80,
                    Location = new Point(10, 10),
                    BackColor = Color.Red,
                };
                var bt_diplay = new Button
                {
                    Text = "afficher",
                    Location = new Point(165, 70),
                    Name = readerP[1].ToString(),
                };
                bt_diplay.Click += new EventHandler(bt_display_CLICK);
                grp_patient.Controls.Add(bt_diplay);
                grp_patient.Controls.Add(lbl_matricule);
                grp_patient.Controls.Add(lbl_Pnom);
                grp_patient.Controls.Add(lbl_nom);
                grp_patient.Controls.Add(picture_patient);

                File_attente.Controls.Add(grp_patient);
                c++;
            }
        }
        private void display_patient(string mat)
        {
            MySqlConnection dbConn = new MySqlConnection("Persist Security Info=False;server=localhost;database=archimed_new;uid=root;password=root");
            MySqlCommand read_patient_info = dbConn.CreateCommand();

            // MessageBox.Show("SELECT * FROM patient WHERE matricule='" + matric + "'");
            read_patient_info.CommandText = "SELECT * FROM patient WHERE matricule='" + mat + "'";

            dbConn.Open();

            MySqlDataReader readerP = read_patient_info.ExecuteReader();
            if (readerP.Read())
            {
                txt_nom.Text = readerP[2].ToString();
                txt_pnom.Text = readerP[3].ToString();
                txt_dat_naiss.Text = readerP[5].ToString();
                txt_telephone.Text = readerP[8].ToString();
                grp_info_patient.Visible = true;
            }
        }
        private void bt_display_CLICK(object sender, EventArgs e)
        {
            Button bouton = sender as Button;
            display_patient(bouton.Name);
        }

    }

    // structure générique
    public struct DEV_BROADCAST_HDR
    {
        public uint dbch_size;
        public uint dbch_devicetype;
        public uint dbch_reserved;
    }

    // structure spécifique
    // notez qu'elle a strictement le même tronche que la générique mais
    // avec des trucs en plus
    public struct DEV_BROADCAST_VOLUME
    {
        public uint dbcv_size;
        public uint dbcv_devicetype;
        public uint dbcv_reserved;
        public uint dbcv_unitmask;
        public ushort dbcv_flags;
    }



}

        
    
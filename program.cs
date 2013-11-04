/*
 * Aplicació CarregaCapsSetmana:  Programa que carregar els caps de setmana a la base de dades de permisos.
 * Llegeix el últim dissabte de la taula de festius calendari 3 i a partir d'aquesta data 
 * carrega un registre nou amb 7 dies més a la taula festius del calendari 3 amb la descripció
 * DISSABTE. El cas del diumenge és idèntic, només canvia el calendari que és el 4 i la descripció.
 * 
 * Autor: March (amb Col·laboració i paciència de David Ezquioga)
 * 
 * Data: Setptembre 2013
 * 
 * Requisits: Llibreries per connectar a Mysql.
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace CarregaCapsSetmana
{
    class Program
    {
        #region Variables
        private const string pathDissabtes = @"c:\\dissabtes.txt";
        private const string pathDiumenges = @"c:\\diumenges.txt";
        
        #endregion

        //Connexió a la base de dades
        public static MySqlConnection connection = new MySqlConnection();
        public static String connectionString;

        public static void Main(string[] args)
        {
            //variable per proves
            DateTime data = new DateTime();
            
            
            //Definim les variables que necessitem
            string text = "";
            DateTime dtDis = new DateTime();
            DateTime dtDiu = new DateTime();
            DateTime dtFi = new DateTime();
            dtFi = DateTime.Today;
            int calDissabte = 3;
            string descDissabte = "DISSABTE";
            int calDiumenge = 4;
            string descDiumenge = "DIUMENGE";

            //Connectem a la base de dades
            iniciaConnexio();

            //comprovem si existeix el fitxer dels dissabtes
            if (System.IO.File.Exists(@pathDissabtes))
            {
                try
                {
                    MySqlCommand instruccio = connection.CreateCommand();
                    //Busquem l'últim dissabte carregat a la base de dades
                    instruccio.CommandText = "SELECT Data FROM Festius where Calendari = 3 ORDER BY Data DESC LIMIT 1";
                    MySqlDataReader reader = instruccio.ExecuteReader();
                    while (reader.Read())
                    {
                        data = Convert.ToDateTime(reader["Data"]);
                        //data = Convert.ToDateTime(reader["Data"].ToString());
                        //Posem el resultat en una variable DateTime amb Parse per que després per tornar-ho a 
                        //carregar a la base de dades no ens doni problemes.
                        //dtDis = DateTime.Parse(reader["Data"].ToString());
                       
                    }

                    dtDis = data;
                    //Copiem el fitxer per seguretat, l'esborrem i en fem un de nou per tenir
                    //la càrrega última controlada
                    copiaFitxer(pathDissabtes);
                    esborraFitxer(pathDissabtes);
                    System.IO.StreamWriter file = new System.IO.StreamWriter(pathDissabtes, true);
                    reader.Dispose();

                    // Per mostrar per pantalla
                    //Console.WriteLine("La data de la consulta és {0}. L'any és. {1}.", dtDis, dtDis.Year);

                    MySqlCommand cmd = connection.CreateCommand();
                    //MySqlCommand cmd = new MySqlCommand();
                    //cmd.Connection = connection;
                    
                    //Carreguem 2 anys de dissabtes a la base de dades
                    while ((dtDis.Year) <= (dtFi.Year + 2))
                    {
                        //afegim 7 dies a la data i estructurem la cadena que volem guardar al fitxer
                        dtDis = dtDis.AddDays(7);
                        text = dtDis.ToShortDateString() + ";" + "3" + ";" + "DISSABTE" + ";";
                        //guardem en fitxer de seguretat
                        file.WriteLine(text);
                        //fem insert a la base de dades mysql
                        cmd.CommandText = "INSERT INTO Festius (Data,Calendari,Descripcio) VALUES ('"+ dtDis.ToString("yyyy-MM-dd")+"',"+ calDissabte + ",'" + descDissabte +"')";
                        cmd.ExecuteNonQuery();
                        // Per mostrar per pantalla
                        //Console.WriteLine(data.ToShortDateString());
                    }

                    // Per mostrar per pantalla
                    Console.WriteLine("La data de la última entrada és {0}. L'any és. {1}.", dtDis, dtDis.Year);
                    Console.WriteLine("Prem any key to tancar.");
                    Console.ReadKey();
                    file.Close();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                connection.Close();
            }
            else
            {
                // NO HI HA FITXER S'HA DE CONTROLAR
                //System.IO.StreamWriter file = new System.IO.StreamWriter(pathDissabtes, true);
                Console.WriteLine("No hi ha fitxer dels dissabtes !. Torneu-ho a provar. Press any key to exit.");
                Console.ReadKey();
                //file.Close();
                connection.Close();
            }

            iniciaConnexio();
            //comprovem si existeix el fitxer dels diumenges
            if (System.IO.File.Exists(@pathDiumenges))
            {
                try
                {
                    MySqlCommand instruccio = connection.CreateCommand();
                    instruccio.CommandText = "SELECT Data FROM Festius where Calendari = 4 ORDER BY Data DESC LIMIT 1";
                    MySqlDataReader reader = instruccio.ExecuteReader();
                    while (reader.Read())
                    {
                        dtDiu = DateTime.Parse(reader["Data"].ToString());
                    }
                    copiaFitxer(pathDiumenges);
                    esborraFitxer(pathDiumenges);
                    System.IO.StreamWriter file = new System.IO.StreamWriter(pathDiumenges, true);
                    reader.Dispose();

                    Console.WriteLine("La data de la consulta és {0}. L'any és. {1}.", dtDiu, dtDiu.Year);

                    MySqlCommand cmd = connection.CreateCommand();

                    while ((dtDiu.Year) <= (dtFi.Year + 2))
                    {
                        //guardem en fitxer de proves
                        dtDiu = dtDiu.AddDays(7);
                        text = dtDiu.ToShortDateString() + ";" + "4" + ";" + "DIUMENGE" + ";";
                        file.WriteLine(text);
                        //fem insert de proves a la base de dades mysql
                        cmd.CommandText = "INSERT INTO Festius (Data,Calendari,Descripcio) VALUES ('" + dtDiu.ToString("yyyy-MM-dd") + "'," + calDiumenge + ",'" + descDiumenge + "')";
                        cmd.ExecuteNonQuery();
                        // Per mostrar per pantalla
                        //Console.WriteLine(data.ToShortDateString());
                    }

                    Console.WriteLine("La data de la última entrada és {0}. L'any és. {1}.", dtDiu, dtDiu.Year);
                    Console.WriteLine("Prem any key to tancar.");
                    Console.ReadKey();

                    file.Close();
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                connection.Close();
            }
            else
            {
                // NO HI HA FITXER S'HA DE CONTROLAR
                //System.IO.StreamWriter file = new System.IO.StreamWriter(pathDiumenges, true);
                Console.WriteLine("No hi ha fitxer dels diumenges !. Torneu-ho a provar. Press any key to exit.");
                Console.ReadKey();
                //file.Close();
                connection.Close();
            }

        }

        private static void iniciaConnexio()
        {
            try
            {
                connectionString = "Server=XXX.XXX.XXX.XXX; Database=permisos ; Uid=permisos; Pwd=********;";
                connection.ConnectionString = connectionString;
                connection.Open();
                Console.WriteLine("Connexió feta amb èxit");
            }
            catch(MySqlException)
            {
                //Cal fer un mail per l'error de connexió
                Console.WriteLine("Error de connexió!");
            }
        }

        //*************************************************************************************//
        //Mètodes auxiliars
        static private DateTime llegeixFitxer(string fitxer)
        {
            int counter = 0;
            string line,lastline;
            lastline = null;
            DateTime dtInici;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(@fitxer);
            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                lastline = line;
                counter++;
            }

            file.Close();
            Console.WriteLine("Ultima data del fitxer: {0}.",lastline);
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.
            //System.Console.ReadLine();
            return dtInici = Convert.ToDateTime(lastline);
        }

        // sense paràmetres -- inicial
        static private DateTime llegeixFitxer()
        {
            int counter = 0;
            string line, lastline;
            lastline = null;
            DateTime dtInici;

            // Read the file and display it line by line.
            System.IO.StreamReader file =
                new System.IO.StreamReader(@"c:\dates.txt");
            while ((line = file.ReadLine()) != null)
            {
                System.Console.WriteLine(line);
                lastline = line;
                counter++;
            }

            file.Close();
            Console.WriteLine("Ultima data del fitxer: {0}.", lastline);
            System.Console.WriteLine("There were {0} lines.", counter);
            // Suspend the screen.
            //System.Console.ReadLine();
            return dtInici = Convert.ToDateTime(lastline);
        }

        private static void copiaFitxer(string fitxer)
        {
            char[] tallar = {'c',':','\\'};
            string fileName = fitxer.TrimStart(tallar);
            string sourcePath = @"C:\";
            string targetPath = @"C:\CopiesDates";

            // Use Path class to manipulate file and directory paths. 
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location: 
            // Create a new target folder, if necessary. 
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and  
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // Keep console window open in debug mode.
            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }


        // sense paràmetres -- inicial
        private static void copiaFitxer()
        {
            string fileName = "dates.txt";
            string sourcePath = @"C:\";
            string targetPath = @"C:\CopiesDates";

            // Use Path class to manipulate file and directory paths. 
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, fileName);

            // To copy a folder's contents to a new location: 
            // Create a new target folder, if necessary. 
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            // To copy a file to another location and  
            // overwrite the destination file if it already exists.
            System.IO.File.Copy(sourceFile, destFile, true);

            // Keep console window open in debug mode.
            //Console.WriteLine("Press any key to exit.");
            //Console.ReadKey();
        }

        private static void esborraFitxer(string fitxer)
        {
            // Delete a file by using File class static method... 
            if (System.IO.File.Exists(@fitxer))
            {
                // Use a try block to catch IOExceptions, to 
                // handle the case of the file already being 
                // opened by another process. 
                try
                {
                    System.IO.File.Delete(@fitxer);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

    }
}

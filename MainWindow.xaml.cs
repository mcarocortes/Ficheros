using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PracticaFicheros_MCARO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Cliente> clientesList = new List<Cliente>();
        StreamWriter ficheroWriter;
        StreamReader ficheroReader;
        bool obligatory1, obligatory2, obligatory4, obligatory5, obligatory8, obligatory9, obligatory10, obligatory11;
        string ficheroDelete = "";

        public MainWindow()
        {
            InitializeComponent();
            if (File.Exists("clientes.txt")) //PRIMERO COMPROBAREMOS SI EXISTE
            {
                //ABRIREMOS FICHERO CLIENTE Y LOS AGREGAREMOS AL ARRAY LIST
                ficheroReader = File.OpenText("clientes.txt");
                int cont = 0;
                string linea;
                string[] provs = { "alicante", "castellón", "valencia" };

                do
                {
                    linea = ficheroReader.ReadLine();

                    if (linea == null) { break; }

                    //LOS AGREGO A MI LISTBOX DE CLIENTES
                    ClientesList.Items.Insert(cont, linea.ToLower());

                    //1.1 AÑADIENDO CLIENTE A ARRAY LIST DE CLIENTES:
                    int i = 0; //creo un contador
                    string nombre = "";
                    string apellido = "";
                    string provincia = ""; // y variables para leer mi clientes.txt

                    //1.2 LEERÉ LINEA POR LINEA Y LES ELIMINARÉ "#"
                    string s = linea.ToLower();  //MI LINEA SERÁ "S"

                    string[] parts = s.Split('#');
                    nombre = parts[0].ToLower();
                    apellido = parts[parts.Length - 2].ToLower(); //LA PENULTIMA
                    provincia = parts[parts.Length - 1].ToLower();//LA ULTIMA PALABRA

                    //1.3 AÑADO CLIENTE A ARRAY LIST DE CLIENTES:
                    Cliente cliente = new Cliente(nombre.ToLower(), apellido.ToLower(), provincia.ToLower());
                    clientesList.Add(cliente);
                    
                    cont++;
                } while (linea != null);
                ficheroReader.Close();
              
            }
            //COMBOBOX DE CREAR CLIENTE, CON LAS OPCIONES DE PROVINCIAS:
            Provincia_Combo5.Items.Add("alicante");
            Provincia_Combo5.Items.Add("castellón");
            Provincia_Combo5.Items.Add("valencia");
            Provincia_Combo4.Items.Add("alicante");
            Provincia_Combo4.Items.Add("castellón");
            Provincia_Combo4.Items.Add("valencia");



        }

        public void Listar(string ficheroName) //MUESTRA CLIENTES Ó PROVINCIAS EN LISTBOX
        {

            //MOSTRAR CONTENIDO DE FICHERO CLIENTE EN LA LISTA DE ITEM
            ficheroReader = File.OpenText(ficheroName + ".txt");
            int cont = 0;
            string linea;
            if (ficheroName == "clientes") { ClientesList.Items.Clear(); } else { ProviList.Items.Clear(); };

            do
            {
                linea = ficheroReader.ReadLine();

                if (linea == null) { break; }
                //DETERMINAMOS A QUE LISTBOX LE AGREAMOS
                if (ficheroName == "clientes") //CLIENTES
                {
                    ClientesList.Items.Insert(cont, linea.ToLower());
                }
                else
                { //PROVINCIA
                    ProviList.Items.Insert(cont, linea.ToLower());
                }

                cont++;
            } while (linea != null);

            ficheroReader.Close();
        }

        //ES PARA MODIFICAR CUALQUIER FICHERO DE PROVINCIA O CLIENTE - EN SECCION CLIENTES:
        public void ModificarFichero(string nomFich, string cambio, int posicion)
        {
            //1. ABRIMOS EL FICHERO DE LECTURA DE PROVINCIA
            ficheroReader = File.OpenText(nomFich + ".txt");

            //2. CREO Y ABRO MI FICHERO VIRTUAL DONDE PASARÉ LA INFORMACIÓN DEL CLIENTE ORIGINAL Y EL FICHERO:
            StreamWriter fichVirtual = File.CreateText("fichVirtual.txt");

            //3. RECORRO LA LISTA DE CLIENTES ORIGINAL Y AGREGO LA INFO AL VIRTUAL
            int contador = 0;
            string linea;
            do
            {
                linea = ficheroReader.ReadLine(); //LEO LÍNEA DEL FICHERO ORIGINAL
                if (linea == null) { break; };

                if (nomFich == "clientes")//ESTA CONDICION APLICA PARA FICHERO CLIENTES
                {

                    if (contador != posicion) // contador != posicion QUE QUEREMOS MODIFICAR
                    {
                        fichVirtual.WriteLine(linea.ToLower()); //LA TRASPASO LA LINEA EN MI FICHERO VIRTUAL
                    }
                    else
                    { // SI LLEGO A LA MISMA LÍNEA QUE DESEO MODIFICAR O ELIMINAR
                      //TRANSCRIBO CAMBIO A ARCHIVO CLIENTE, PUEDE SER "" Ó {0}#{1}#{2}
                        fichVirtual.WriteLine(cambio);
                    }
                }
                else
                { //SINO, SERÁ UN FICHERO PROVINCIA   
                    if ((clientesList[posicion].Nombre + " " + clientesList[posicion].Apellido) == linea)//YO SE QUE QUIERO CAMBIAR ESTE VALOR
                    {
                        fichVirtual.WriteLine(cambio);
                    }
                    else
                    {
                        fichVirtual.WriteLine(linea.ToLower());
                    }
                };
                contador++;
            } while (linea != null);
            ficheroReader.Close();
            fichVirtual.Close();

            //4. OPERACIÓN INVERSA / INFO DEL VIRTUAL AL ORIGINAL
            //4.1. HAREMOS LO MISMO CON CLIENTE ABRIMOS EL FICHERO DE LECTURA_ORIGINAL VIRTUAL

            int i = 0;
            //vacia = false;
            StreamReader virtualReader = File.OpenText("fichVirtual.txt");
            //4.2. ABRIMOS EL FICHERO DE ESCRITURA PROVINCIAS
            using (StreamWriter ficheroNuevo = new StreamWriter(nomFich + ".txt", false)) //ABRO ARCHIVO ORIGINAL Y SOBREESCRIBO
            {
                do
                {
                    linea = virtualReader.ReadLine(); //LEO LÍNEA DE CLIENTE VIRTUAL
                    if (linea == null) { break; };

                    if ((linea != ""))
                    {  // ES DIFERENTE A UNA VACÍA
                        ficheroNuevo.WriteLine(linea.ToLower()); // LA SOBRESCRIBO EN CLIENTE.TXT
                    }
                    i++;
                } while (linea != null);
            }
            virtualReader.Close();
            File.Delete("fichVirtual.txt");
        }


        //ES PARA MODIFICAR CUALQUIER FICHERO DE PROVINCIA - EN SECCION PROVINCIA:
        public void ModificarFicheroProv(string nomFich, string cambio, int posicion)
        {
            //1. ABRIMOS EL FICHERO DE LECTURA DE PROVINCIA
            ficheroReader = File.OpenText(nomFich + ".txt");

            //2. CREO Y ABRO MI FICHERO VIRTUAL DONDE PASARÉ LA INFORMACIÓN DEL CLIENTE ORIGINAL Y EL FICHERO:
            StreamWriter fichVirtual = File.CreateText("fichVirtual.txt");

            //3. RECORRO LA LISTA DE CLIENTES ORIGINAL Y AGREGO LA INFO AL VIRTUAL
            int contador = 0;
            string linea;
            do
            {
                linea = ficheroReader.ReadLine(); //LEO LÍNEA DEL FICHERO ORIGINAL
                if (linea == null) { break; };
                if (contador == posicion)//CUANDO LLEGUE A LA LÍNEA DESEADA
                {
                    fichVirtual.WriteLine(cambio);
                }
                else
                {
                    fichVirtual.WriteLine(linea.ToLower());
                }
                contador++;
            } while (linea != null);
            ficheroReader.Close();
            fichVirtual.Close();

            //4. OPERACIÓN INVERSA / INFO DEL VIRTUAL AL ORIGINAL
            //4.1. HAREMOS LO MISMO CON CLIENTE ABRIMOS EL FICHERO DE LECTURA_ORIGINAL VIRTUAL

            int i = 0;
            //vacia = false;
            StreamReader virtualReader = File.OpenText("fichVirtual.txt");
            //4.2. ABRIMOS EL FICHERO DE ESCRITURA PROVINCIAS
            using (StreamWriter ficheroNuevo = new StreamWriter(nomFich + ".txt", false)) //ABRO ARCHIVO ORIGINAL Y SOBREESCRIBO
            {
                do
                {
                    linea = virtualReader.ReadLine(); //LEO LÍNEA DE CLIENTE VIRTUAL
                    if (linea == null) { break; };

                    if ((linea != ""))
                    {  // ES DIFERENTE A UNA VACÍA
                        ficheroNuevo.WriteLine(linea.ToLower()); // LA SOBRESCRIBO EN CLIENTE.TXT
                    }
                    i++;
                } while (linea != null);
            }
            virtualReader.Close();
            File.Delete("fichVirtual.txt");
        }

        private void List_Selected(object sender, SelectionChangedEventArgs e)
        {
            ListBox lista = sender as ListBox;
            switch (lista.Name)
            {
                case "ClientesList":
                    //HABILITO BOTON DE ELIMINAR
                    EliminarBtn.IsEnabled = true;
                    //HABILITO TEXTBLOCK DE MODIFICAR
                    Provincia_Combo4.IsEnabled = true;

                    break;

                case "ProviList":
                  //HABILITO BOTON DE ELIMINAR CLIENTE DE PROVINCIA
                    if (ProviList.SelectedIndex != -1)
                    {
                        EliminarBtn_Prov.IsEnabled = true;
                        Provincia_Combo3.IsEnabled = true;
                    }
                    else
                    {
                        EliminarBtn_Prov.IsEnabled = false;
                        Provincia_Combo3.SelectedIndex = -1;
                        Provincia_Combo3.IsEnabled = false;
                        ModificarBtn_Prov.IsEnabled = false;
                    }
                    break;
            }
        }

        private void Selection_Combo(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;

            switch (combo.Name)
            {
                case "Provincia_Combo":
                    ProviList.SelectedIndex = -1;
                    if (combo.SelectedIndex != -1) { Listar(combo.SelectedItem.ToString()); };
                    break;

                case "Provincia_Combo2":
                    NameText_Prov.IsEnabled = true;
                    SurnameText_Prov.IsEnabled = true;
                    if ((obligatory10 == true) && (obligatory11 == true))
                    { if (Provincia_Combo3.SelectedIndex != -1) { ModificarBtn_Prov.IsEnabled = true; } }
                    else { ModificarBtn_Prov.IsEnabled = false; }
                    break;



                case "Provincia_Combo3":
                    NameText_ProvMod.IsEnabled = true;
                    SurnameText_ProvMod.IsEnabled = true;
                    if ((obligatory10 == true) && (obligatory11 == true))
                    { if (Provincia_Combo3.SelectedIndex != -1) { ModificarBtn_Prov.IsEnabled = true; } }
                    else { ModificarBtn_Prov.IsEnabled = false; }
                    break;

                case "Provincia_Combo4":
                    NameText_Mod.IsEnabled = true;
                    SurnameText_Mod.IsEnabled = true;
                    if ((obligatory4 == true) && (obligatory5 == true))
                    { if (Provincia_Combo4.SelectedIndex != -1) { ModificarBtn.IsEnabled = true; } }
                    else { ModificarBtn.IsEnabled = false; }
                    break;

                case "Provincia_Combo5":
                    if ((obligatory1 == true) && (obligatory2 == true))
                    { if (Provincia_Combo5.SelectedIndex != -1) { GenerarBtn.IsEnabled = true; } }
                    else { GenerarBtn.IsEnabled = false; }
                    break;


            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextBox textos = sender as TextBox;

            //*****RESTRICCIONES PARA RELLENAR TODOS LOS CAMPOS DE FORMULARIO CLIENTES
            switch (textos.Name)
            {
                //CAMPOS OBLIGATORIOS EN FORMULARIO CLIENTE
                case "NameText": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory1 = false; } else { obligatory1 = true; }; break;
                case "SurnameText": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory2 = false; } else { obligatory2 = true; }; break;

                //CAMPOS OBLIGATORIOS EN MODIFICACION CLIENTE + SELECCION DE CLIENTE EN LISTA
                case "NameText_Mod": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory4 = false; } else { obligatory4 = true; }; break;
                case "SurnameText_Mod": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory5 = false; } else { obligatory5 = true; }; break;

                //CAMPOS OBLIGATORIOS EN AÑADIR CLIENTES DE PROVINCIA
                case "NameText_Prov": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory8 = false; } else { obligatory8 = true; }; break;
                case "SurnameText_Prov": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory9 = false; } else { obligatory9 = true; }; break;

                //CAMPOS OBLIGATORIOS EN MODIFICAR CLIENTES DE PROVINCIA
                case "NameText_ProvMod": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory10 = false; } else { obligatory10 = true; }; break;
                case "SurnameText_ProvMod": if ((textos.Text == null) || (textos.Text == " ") || (textos.Text == "")) { obligatory11 = false; } else { obligatory11 = true; }; break;


            }

            //*****HABILITAR BOTON PARA GENERAR CLIENTE
            if ((obligatory1 == true) && (obligatory2 == true))
            { if (Provincia_Combo5.SelectedIndex != -1) { GenerarBtn.IsEnabled = true; } }
            else { GenerarBtn.IsEnabled = false; }

            //*****HABILITAR BOTON PARA MODIFICAR CLIENTE
            if ((obligatory4 == true) && (obligatory5 == true) && (Provincia_Combo4.SelectedIndex != -1))
            { ModificarBtn.IsEnabled = true; }
            else { ModificarBtn.IsEnabled = false; }

            //** HABILITAR BOTON DE AÑADIR CLIENTE A PROVINCIA
            if ((obligatory8 == true) && (obligatory9 == true) && (Provincia_Combo2.SelectedIndex != -1))
            { Añadir_BtnProv.IsEnabled = true; }
            else { Añadir_BtnProv.IsEnabled = false; }

            //** HABILITAR BOTON DE MODIFICAR CLIENTE DE PROVINCIA
            if ((obligatory10 == true) && (obligatory11 == true) && (Provincia_Combo3.SelectedIndex != -1))
            { ModificarBtn_Prov.IsEnabled = true; }
            else { ModificarBtn_Prov.IsEnabled = false; }
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            Button boton = sender as Button;
            int opcion = 0;
            int contadorDelete;
            bool vacio;
            switch (boton.Name)
            {
                case "GenerarBtn":
                    //1.AÑADO CLIENTE A ARRAY LIST DE CLIENTES:
                    Cliente cliente = new Cliente(NameText.Text.ToLower(), SurnameText.Text.ToLower(), Provincia_Combo5.SelectedItem.ToString().ToLower());
                    clientesList.Add(cliente);

                    //AÑADIRÉ A CLIENTES.txt EL CONTENIDO DE FORMULARIO
                    ficheroWriter = File.AppendText("clientes.txt"); //ABRO ARCHIVO CLIENTES
                    ficheroWriter.WriteLine("{0}#{1}#{2}", NameText.Text.ToLower(), SurnameText.Text.ToLower(), Provincia_Combo5.SelectedItem.ToString().ToLower());
                    ficheroWriter.Close();
                 
                    break;
                case "ModificarBtn":
                    ficheroDelete = clientesList[ClientesList.SelectedIndex].Provincia.ToLower(); //NOMBRE DE FICHERO A BORRAR, SÓLO SI ESTA VACIO
                  
                    //AGREGO A LA LISTA DE CLIENTES : EL VALOR MODIFICADO EN LA POSICION SELECCIONADA
                    clientesList[ClientesList.SelectedIndex] = new Cliente(NameText_Mod.Text.ToLower(), SurnameText_Mod.Text.ToLower(), Provincia_Combo4.SelectedItem.ToString());
                    //MODIFICO FICHERO CLIENTES CON LA INFORMACION NUEVA
                    ModificarFichero("clientes", NameText_Mod.Text.ToLower() + "#" + SurnameText_Mod.Text.ToLower() + "#" + Provincia_Combo4.SelectedItem.ToString(), ClientesList.SelectedIndex);


                    break;
                case "EliminarBtn":
                    ficheroDelete = clientesList[ClientesList.SelectedIndex].Provincia.ToLower();                  
                    //ELIMINO A LA LISTA DE CLIENTES : EL VALOR MODIFICADO EN LA POSICION SELECCIONADA
                    clientesList.RemoveAt(ClientesList.SelectedIndex);
                    //MODIFICO FICHERO CLIENTES CON LA INFORMACION NUEVA
                    ModificarFichero("clientes", "", ClientesList.SelectedIndex);
                    break;

                case "Añadir_BtnProv":
                    ficheroWriter = File.AppendText(Provincia_Combo2.SelectedItem + ".txt");
                    ficheroWriter.WriteLine("{0} {1}", NameText_Prov.Text.ToLower(), SurnameText_Prov.Text.ToLower());
                    ficheroWriter.Close();

                    if (Provincia_Combo.SelectedIndex != -1)
                    { //SI HAY ALGUNA PROVINCIA SELECCIONADA LA MOSTRARÁ
                        Listar(Provincia_Combo.SelectedItem.ToString());
                    }
                    break;
                case "ModificarBtn_Prov":
                    // SI LA PROVINCIA SELECCIONADA ES != AL VALOR DE LA PROVINCIA ORIGINAL
                    if (Provincia_Combo3.SelectedItem.ToString() != Provincia_Combo.SelectedItem.ToString())
                    {
                        // ABRO EL FICHERO PROVINCIA AL QUE PERTENECE Y LO AGREGO
                        StreamWriter proviAgregado = File.AppendText(Provincia_Combo3.SelectedItem.ToString() + ".txt");
                        proviAgregado.WriteLine("{0} {1}", NameText_ProvMod.Text.ToLower(), SurnameText_ProvMod.Text.ToLower());
                        proviAgregado.Close();
                        //MODIFICO FICHERO PROVINCIA ORIGEN: BORRO: escribo "" CLIENTE DEL ORIGEN, PORQUE CAMBIÉ DE PROVINCIA
                        //  NOMBRE DEL FICHERO ORIGEN                         //MODIF // POSICION
                        ModificarFicheroProv(Provincia_Combo.SelectedItem.ToString(), "", ProviList.SelectedIndex);
                    }
                    else
                    { // SI ES LA MISMA PROVINCIA//MODIFICO FICHERO PROVINCIA ORIGEN: CAMBIO NOMBRE Y APELLIDO 
                        //  NOMBRE DEL FICHERO                            //MODIF                                                                // POSICION
                        ModificarFicheroProv(Provincia_Combo.SelectedItem.ToString(), NameText_ProvMod.Text.ToLower() + " " + SurnameText_ProvMod.Text.ToLower(), ProviList.SelectedIndex);
                    };
                    break;
                case "EliminarBtn_Prov":
                    //MODIFICO FICHERO PROVINCIA ORIGEN: BORRO: escribo "" CLIENTE DEL ORIGEN, PORQUE CAMBIÉ DE PROVINCIA
                    //  NOMBRE DEL FICHERO                          //MODIF // POSICION
                    ModificarFicheroProv(Provincia_Combo.SelectedItem.ToString(), "", ProviList.SelectedIndex);
                    break;
              
                case "CrearFich_Btn":
                    Provincia_Combo.Items.Clear();
                    Provincia_Combo2.Items.Clear();
                    Provincia_Combo3.Items.Clear();

                    List<string> provinciasTotales = new List<string>(); // CREARE UNA LISTA CON LAS PROVINCIAS EXISTENTES

                    for (int i = 0; i < clientesList.Count; i++) {
                        provinciasTotales.Add(clientesList[i].Provincia.ToLower()); //LAS AÑADIRÉ AL ARRAYLIST
                    }

                    List<string> distinct = provinciasTotales.Distinct().ToList(); // LES ELIMINARÉ LAS DUPLICADAS 

                    for (int i = 0; i < distinct.Count; i++) { //LAS AGREGARÉ AL COMBOBOS LAS UNICAS EXISTENTES
                        Provincia_Combo.Items.Add(distinct[i].ToLower());
                        Provincia_Combo2.Items.Add(distinct[i].ToLower());
                        Provincia_Combo3.Items.Add(distinct[i].ToLower());
                    }

                    Provincia_Combo2.IsEnabled = true;
                    Provincia_Combo.IsEnabled = true;
                    Provincia_Combo.SelectedIndex = -1;
                    
                    //1.2 FICHEROS: UNA VEZ CREADO EL COMBOBOX DE PROVINCIAS EXISTENTES, CREARÉ LOS ARCHIVOS CORRESPONDIENTES AL NOMBRE DE PROV

                    for (int j = 0; j < Provincia_Combo.Items.Count; j++) // TENGO MI ARRAY DE PROVINCIAS ÚNICAS 
                    {
                        if (File.Exists(Provincia_Combo.Items[j].ToString().ToLower() + ".txt")) //PRIMERO COMPROBAREMOS SI EXISTE
                        {
                            using (StreamWriter ficheroNuevo = new StreamWriter(Provincia_Combo.Items[j].ToString().ToLower() + ".txt", false)) //ABRO ARCHIVO ORIGINAL Y SOBREESCRIBO
                            {
                                for (int i = 0; i < clientesList.Count; i++) //LEEREMOS TODA LA LISTA DE CLIENTES
                                {

                                    if (clientesList[i].Provincia.ToLower() == Provincia_Combo.Items[j].ToString().ToLower()) //CUANDO LA PROVINCIA DEL CLIENTE SEA = AL NOMBRE DEL TXT
                                        ficheroNuevo.WriteLine(clientesList[i].Nombre + " " + clientesList[i].Apellido); // LA SOBRESCRIBO EN fichero.TXT                                   
                                };
                            }
                        }
                        else
                        { //SI MI FICHERO NO EXISTE
                            StreamWriter ficheroNuevo = File.CreateText(Provincia_Combo.Items[j].ToString().ToLower() + ".txt");//LO CREO
                            for (int i = 0; i < clientesList.Count; i++) //LEEREMOS TODA LA LISTA DE CLIENTES
                            {
                                if (clientesList[i].Provincia == Provincia_Combo.Items[j].ToString().ToLower()) //CUANDO LA PROVINCIA DEL CLIENTE SEA = AL TXT
                                    ficheroNuevo.WriteLine(clientesList[i].Nombre + " " + clientesList[i].Apellido); // LA SOBRESCRIBO EN fichero.TXT                                   
                            };
                            ficheroNuevo.Close();
                        }
                    };

                   
                    ficheroDelete = "";
                    ProviList.Items.Clear();
                    MessageBox.Show("¡Ficheros Creados!");

                    break;

                case "x":
                    Close();
                    break;
            }

            // LISTAMOS CLIENTES EN CLIENTLISTBOX
            Listar("clientes");

            if (Provincia_Combo.SelectedIndex != -1) { Listar(Provincia_Combo.SelectedItem.ToString()); }

            //LIMPIO TODAS LAS CASILLAS 
            NameText.Clear();
            SurnameText.Clear();
            NameText_Mod.Clear();
            SurnameText_Mod.Clear();
            NameText_Prov.Clear();
            SurnameText_Prov.Clear();
            NameText_ProvMod.Clear();
            SurnameText_ProvMod.Clear();

            //OPERACIONES EN CLIENTE
            //BLOQUEO EL BOTONES Y TEXBLOCK, HASTA QUE SE CLICKE NUEVAMENTE OTRO ELEMENTO DE LA LISTA
            EliminarBtn.IsEnabled = false;
            NameText_Mod.IsEnabled = false;
            SurnameText_Mod.IsEnabled = false;


            //OPERACIONES EN CLIENTE PROVINCIA
            //BLOQUEO EL BOTONES Y TEXBLOCK, HASTA QUE SE CLICKE NUEVAMENTE OTRO ELEMENTO DE LA LISTA         
            Provincia_Combo5.SelectedIndex = -1;
            Provincia_Combo4.IsEnabled = false;
            Provincia_Combo4.SelectedIndex = -1;
            Provincia_Combo2.SelectedIndex = -1;
            Provincia_Combo3.IsEnabled = false;
            Provincia_Combo3.SelectedIndex = -1;


            NameText_Mod.IsEnabled = false;
            SurnameText_Mod.IsEnabled = false;
            EliminarBtn_Prov.IsEnabled = false;
            NameText_Prov.IsEnabled = false;
            SurnameText_Prov.IsEnabled = false;
            NameText_ProvMod.IsEnabled = false;
            SurnameText_ProvMod.IsEnabled = false;

            //CONDICION PARA ELIMINAR DEL COMBOBOX Y FICHERO VACIO
            //BUSCARÉ EN MI ARRAY DE CLIENTES SI EXISTE LA PROVINCIA, MODIFICADA.
            int existe = 0;
            for (int i = 0; i < clientesList.Count; i++)
            {
                if (clientesList[i].Provincia == ficheroDelete)
                {
                    existe++;
                }
            }
            if (existe == 0 && ficheroDelete != "") //SI NO EXISTEEN MI ARRAYLIST DE CLIENTES
            {
                if(File.Exists(ficheroDelete + ".txt")){ 
                MessageBox.Show("Se Eliminará Fichero:" + ficheroDelete + ".txt");
                Provincia_Combo.Items.Remove(ficheroDelete); //LOS ELIMINARÉ DE COMBOBOX
                Provincia_Combo2.Items.Remove(ficheroDelete);
                Provincia_Combo3.Items.Remove(ficheroDelete);
                File.Delete(ficheroDelete + ".txt");
                };
            }
            ficheroDelete = "";


        }
    }
}


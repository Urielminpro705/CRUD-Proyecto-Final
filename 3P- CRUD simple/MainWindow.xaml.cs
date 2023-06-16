using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3P__CRUD_simple
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Mostraralumnos();
            ocultar();
        }

        private int[] indice = new int[100];
        private SqlConnection conectateSQL = new SqlConnection("Data Source=DESKTOP-D3BA5M2\\SQLEXPRESS;Initial Catalog=Alumnos;Integrated Security=True");


        public void Mostraralumnos()
        {
            object c1, c2, c3;
            string consulta = "SELECT * FROM alumnos;";
            SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conectateSQL);
            DataSet datos = new DataSet();
            conectateSQL.Open();
            adaptador.Fill(datos);
            try
            {
                DataTable tabla = datos.Tables[0];
                tabla.Columns.Add("promedio");
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    DataRow fila = tabla.Rows[i];
                    indice[i] = (int)fila["id"];
                    c1 = fila["cal1"];
                    c2 = fila["cal2"];
                    c3 = fila["cal3"];
                    fila["promedio"] =Math.Round(((double)c1 + (double)c2 + (double)c3) / 3, 1);


                }
                lista.ItemsSource = tabla.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            conectateSQL.Close();
        }

        private void borrar_Click(object sender, RoutedEventArgs e)
        {
            if (lista.SelectedIndex != -1)
            {
                string consulta = "DELETE FROM alumnos WHERE id=@elID";
                SqlCommand miComandoSQL = new SqlCommand(consulta, conectateSQL);
                conectateSQL.Open();
                miComandoSQL.Parameters.AddWithValue("@elID", indice[lista.SelectedIndex]);
                miComandoSQL.ExecuteNonQuery();
                conectateSQL.Close();
                Mostraralumnos();
            }                                    
        }


        private void agregar_Click(object sender, RoutedEventArgs e)
        {
            if(nombreN.Text == "" || apellidoPN.Text == "" || apellidoMN.Text == "")
            {
                MessageBox.Show("Faltan datos necesarios");
            }
            else
            {
                if(c1N.Text == "" || c2N.Text == "" || c3N.Text == "")
                {
                    c1N.Text = "0";
                    c2N.Text = "0";
                    c3N.Text = "0";
                }
                string consulta = "INSERT INTO alumnos (nombre, apellidoP, apellidoM, cal1, cal2, cal3) VALUES (@n, @ap, @am, @c1, @c2, @c3)";
                SqlCommand miComandoI = new SqlCommand(consulta, conectateSQL);
                conectateSQL.Open();
                try
                {                    
                    miComandoI.Parameters.AddWithValue("@n", nombreN.Text);
                    miComandoI.Parameters.AddWithValue("@ap", apellidoPN.Text);
                    miComandoI.Parameters.AddWithValue("@am", apellidoMN.Text);
                    miComandoI.Parameters.AddWithValue("@c1", double.Parse(c1N.Text));
                    miComandoI.Parameters.AddWithValue("@c2", double.Parse(c2N.Text));
                    miComandoI.Parameters.AddWithValue("@c3", double.Parse(c3N.Text));
                    miComandoI.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                conectateSQL.Close();
                Mostraralumnos();
                nombreN.Text = "";
                apellidoPN.Text = "";
                apellidoMN.Text = "";
                c1N.Text = "";
                c2N.Text = "";
                c3N.Text = "";
            }    
        }

        private void aceptar_Click(object sender, RoutedEventArgs e)
        {
            int elIDOriginal = (int)indice[lista.SelectedIndex];
            string consulta = "UPDATE alumnos SET nombre= @n, apellidoP = @ap, apellidoM = @am, cal1 = @c1, cal2 = @c2, cal3 = @c3 WHERE id = " + elIDOriginal;
            SqlCommand miComandoI = new SqlCommand(consulta, conectateSQL);
            conectateSQL.Open();
            try
            {
                miComandoI.Parameters.AddWithValue("@n", nombreN.Text);
                miComandoI.Parameters.AddWithValue("@ap", apellidoPN.Text);
                miComandoI.Parameters.AddWithValue("@am", apellidoMN.Text);
                miComandoI.Parameters.AddWithValue("@c1", c1N.Text);
                miComandoI.Parameters.AddWithValue("@c2", c2N.Text);
                miComandoI.Parameters.AddWithValue("@c3", c3N.Text);
                miComandoI.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conectateSQL.Close();
            ocultar();
            activar();
            Mostraralumnos();
        }

        private void desactivar()
        {
            lista.IsEnabled = false;
            actub.Visibility = Visibility.Hidden;
            borrarb.Visibility = Visibility.Hidden;
            borrarTodob.Visibility = Visibility.Hidden;
        }

        private void activar()
        {
            lista.IsEnabled = true;
            actub.Visibility = Visibility.Visible;
            borrarb.Visibility = Visibility.Visible;
            borrarTodob.Visibility = Visibility.Visible;
        }

        private void actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                desactivar();
                agregarb.Visibility = Visibility.Hidden;
                string consulta = "SELECT * FROM alumnos WHERE id = @elID";
                SqlCommand miComandoSQL = new SqlCommand(consulta, conectateSQL);
                SqlDataAdapter adaptador = new SqlDataAdapter(miComandoSQL);
                DataSet datos = new DataSet();
                using (adaptador)
                {
                    miComandoSQL.Parameters.AddWithValue("@elID", indice[lista.SelectedIndex]);
                    adaptador.Fill(datos);
                    DataTable tabla = datos.Tables[0];
                    nombreN.Text = tabla.Rows[0]["nombre"].ToString();
                    apellidoPN.Text = tabla.Rows[0]["apellidoP"].ToString();
                    apellidoMN.Text = tabla.Rows[0]["apellidoM"].ToString();
                    c1N.Text = tabla.Rows[0]["cal1"].ToString();
                    c2N.Text = tabla.Rows[0]["cal2"].ToString();
                    c3N.Text = tabla.Rows[0]["cal3"].ToString();
                }
                aceptarb.Visibility = Visibility.Visible;
                cancelarb.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        private void lista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lista.SelectedIndex != -1)
            {
                actub.Visibility = Visibility.Visible;
                borrarb.Visibility = Visibility.Visible;

            }
            else
            {
                actub.Visibility = Visibility.Hidden;
                borrarb.Visibility = Visibility.Hidden;
            }
        }

        private void cancelar_Click(object sender, RoutedEventArgs e)
        {
            activar();
            ocultar();
        }

        public void ocultar()
        {
            agregarb.Visibility = Visibility.Visible;
            aceptarb.Visibility = Visibility.Collapsed;
            cancelarb.Visibility = Visibility.Collapsed;
            nombreN.Text = "";
            apellidoPN.Text = "";
            apellidoMN.Text = "";
            c1N.Text = "";
            c2N.Text = "";
            c3N.Text = "";
        }

        private void borrarDatos_Click(object sender, RoutedEventArgs e)
        {
            string consulta = "DELETE FROM alumnos;";
            SqlCommand miComandoSQL = new SqlCommand(consulta, conectateSQL);
            conectateSQL.Open();
            try
            {
                miComandoSQL.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            conectateSQL.Close();

            Mostraralumnos();
        }
    }
}

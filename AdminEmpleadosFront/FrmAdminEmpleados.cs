﻿using AdminEmpleadosEntidades;
using AdminEmpleadosNegocio;
using System.Reflection.Metadata;

namespace AdminEmpleadosFront
{
    public partial class FrmAdminEmpleados : Form
    {
        List<Empleado> empleadosList = new List<Empleado>();
        List<Empleado> listaAnular = new List<Empleado>();

        public FrmAdminEmpleados()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            buscarEmpleados();
        }
        private List<Empleado> buscarEmpleados()
        {
            //Obtengo el nombre y DNI ingresado por el usuario
            string textoBuscar = txtBuscar.Text.Trim().ToUpper();

            //declaro el parametro
            Empleado parametro = new Empleado();

            //asigno el nombre ingresado
            if (!String.IsNullOrEmpty(textoBuscar.Trim()))
            {
                parametro.Nombre = textoBuscar;
                parametro.Dni = textoBuscar;
            }

            //Busco la lista de empleados en la capa de negocio, pasandole el parametro ingresado
            if (checkBox1.Checked)
            {
                empleadosList = EmpleadosNegocio.GetTotal(parametro);
                listaAnular = EmpleadosNegocio.GetAnulados(parametro);

            }
            else
                empleadosList = EmpleadosNegocio.Get(parametro);
            //Actualizo la grilla
            refreshGrid();
            return empleadosList;
        }

        private void refreshGrid()
        {
            //Actualizo el Binding con la lista de empleados que viene desde la BD
            empleadoBindingSource.DataSource = null;
            empleadoBindingSource.DataSource = empleadosList;

        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Llamo al metodo buscar al presionar la tecla "Enter"
            if (e.KeyChar == (char)Keys.Enter)
            {
                buscarEmpleados();
            }
        }

        private void btnAlta_Click(object sender, EventArgs e)
        {
            FrmEditEmpleados frm = new FrmEditEmpleados();

            frm.modo = EnumModoForm.Alta;
            frm.ShowDialog();//modal

            buscarEmpleados();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (empleadoBindingSource.Current == null)
                return;

            FrmEditEmpleados frm = new FrmEditEmpleados();

            frm.modo = EnumModoForm.Modificacion;
            frm._empleado = (Empleado)empleadoBindingSource.Current;

            frm.ShowDialog();

            buscarEmpleados();
        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            if (empleadoBindingSource.Current == null)
                return;

            FrmEditEmpleados frm = new FrmEditEmpleados();

            frm.modo = EnumModoForm.Consulta;
            frm._empleado = (Empleado)empleadoBindingSource.Current;

            frm.ShowDialog();

            buscarEmpleados();
        }

        private void btnBaja_Click(object sender, EventArgs e)
        {
            if (empleadoBindingSource.Current == null)
                return;

            Empleado emp = (Empleado)empleadoBindingSource.Current;

            //pregunto si quiere guardar los datos
            DialogResult res = MessageBox.Show("¿Confirma anular el empleado " + emp.Nombre + " ?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No)
            {
                return;
            }

            try
            {
                EmpleadosNegocio.Anular((int)emp.EmpleadoId);
                MessageBox.Show("El empleado " + emp.Nombre + " se anuló correctamente", "Anulación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            buscarEmpleados();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            empleadosList = buscarEmpleados();
            DialogResult res = MessageBox.Show("¿Desea Eliminar los Empleados Anulados ?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.No)
            {
                return;
            }

            try
            {
                DialogResult res2 = MessageBox.Show("¿Conforma Eliminar definitivamente los Empleados Anulados de la Base de Datos?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res2 == DialogResult.No)
                {
                    return;
                }
                try
                {
                    EmpleadosNegocio.EliminarAnulados(listaAnular);
                    MessageBox.Show("Se Eliminaron correctamente los Empleados Anulados", "Anulación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    checkBox1.Checked = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            buscarEmpleados();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            buscarEmpleados();
        }
    }
}

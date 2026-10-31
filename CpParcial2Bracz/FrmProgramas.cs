using CadParcial2Bracz;
using ClnParcial2Bracz;
using System;
using System.Windows.Forms;

namespace CpParcial2Bracz
{
    public partial class FrmProgramas : Form
    {
        private const string TituloMensaje = "::: Mensaje - Parcial 2 :::";
        private const string TituloError = "::: Error - Parcial 2 :::";
        private const string FormatoHora = @"hh\:mm";

        private bool esNuevo;

        public FrmProgramas()
        {
            InitializeComponent();
        }

        private void FrmProgramas_Load(object sender, EventArgs e)
        {
            cargarCanales();
            cargarDiasSemana();
            listar();
            habilitarEdicion(false);
        }

        // ------------------------- Carga de datos -------------------------

        private void cargarCanales()
        {
            cbxCanal.ValueMember = "id";
            cbxCanal.DisplayMember = "nombre";
            cbxCanal.DataSource = CanalCln.listar();
            cbxCanal.SelectedIndex = -1;
        }

        private void cargarDiasSemana()
        {
            cbxDiaSemana.Items.AddRange(Constantes.DiasSemana);
            cbxDiaSemana.SelectedIndex = -1;
        }

        private void listar()
        {
            var programas = HorarioProgramaCln.listar(txtParametro.Text);
            dgvLista.DataSource = programas;
            dgvLista.Columns["id"].Visible = false;
            dgvLista.Columns["idCanal"].Visible = false;
            dgvLista.Columns["canal"].HeaderText = "Canal";
            dgvLista.Columns["titulo"].HeaderText = "Título";
            dgvLista.Columns["horaInicio"].HeaderText = "Hora Inicio";
            dgvLista.Columns["horaInicio"].DefaultCellStyle.Format = FormatoHora;
            dgvLista.Columns["horaFin"].HeaderText = "Hora Fin";
            dgvLista.Columns["horaFin"].DefaultCellStyle.Format = FormatoHora;
            dgvLista.Columns["diaSemana"].HeaderText = "Día";
            dgvLista.Columns["conductorPrincipal"].HeaderText = "Conductor Principal";

            if (programas.Count > 0) dgvLista.CurrentCell = dgvLista.Rows[0].Cells["titulo"];
            btnEditar.Enabled = programas.Count > 0;
            btnEliminar.Enabled = programas.Count > 0;
        }

        // ----------------------------- Búsqueda ----------------------------

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        private void txtParametro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter) listar();
        }

        // ---------------------------- Estado de UI ---------------------------

        private void habilitarEdicion(bool editando)
        {
            gbxDatos.Enabled = editando;
            gbxLista.Enabled = !editando;
            pnlAcciones.Enabled = !editando;
            txtParametro.Enabled = !editando;
            btnBuscar.Enabled = !editando;
        }

        private void resetearCampos()
        {
            cbxCanal.SelectedIndex = -1;
            cbxDiaSemana.SelectedIndex = -1;
            txtTitulo.Clear();
            txtConductor.Clear();
            dtpHoraInicio.Value = DateTime.Today.AddHours(9);
            dtpHoraFin.Value = DateTime.Today.AddHours(10);
            erpError.Clear();
        }

        private int obtenerIdSeleccionado()
        {
            return (int)dgvLista.CurrentRow.Cells["id"].Value;
        }

        private static TimeSpan obtenerHora(DateTimePicker picker)
        {
            return new TimeSpan(picker.Value.Hour, picker.Value.Minute, 0);
        }

        // ------------------------------ CRUD ------------------------------

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            esNuevo = true;
            resetearCampos();
            habilitarEdicion(true);
            cbxCanal.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            esNuevo = false;
            resetearCampos();

            var programa = HorarioProgramaCln.obtenerUno(obtenerIdSeleccionado());
            cbxCanal.SelectedValue = programa.idCanal;
            cbxDiaSemana.SelectedItem = programa.diaSemana;
            txtTitulo.Text = programa.titulo;
            txtConductor.Text = programa.conductorPrincipal;
            dtpHoraInicio.Value = DateTime.Today.Add(programa.horaInicio);
            dtpHoraFin.Value = DateTime.Today.Add(programa.horaFin);

            habilitarEdicion(true);
            txtTitulo.Focus();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            erpError.Clear();
            habilitarEdicion(false);
        }

        private bool validarCampos()
        {
            bool esValido = true;
            erpError.Clear();

            if (cbxCanal.SelectedIndex == -1)
            {
                erpError.SetError(cbxCanal, "El canal es obligatorio");
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                erpError.SetError(txtTitulo, "El título es obligatorio");
                esValido = false;
            }
            if (cbxDiaSemana.SelectedIndex == -1)
            {
                erpError.SetError(cbxDiaSemana, "El día de la semana es obligatorio");
                esValido = false;
            }
            if (obtenerHora(dtpHoraFin) <= obtenerHora(dtpHoraInicio))
            {
                erpError.SetError(dtpHoraFin, "La hora de fin debe ser mayor a la hora de inicio");
                esValido = false;
            }
            if (string.IsNullOrWhiteSpace(txtConductor.Text))
            {
                erpError.SetError(txtConductor, "El conductor principal es obligatorio");
                esValido = false;
            }

            return esValido;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validarCampos()) return;

            var programa = new HorarioPrograma
            {
                idCanal = (int)cbxCanal.SelectedValue,
                titulo = txtTitulo.Text.Trim(),
                horaInicio = obtenerHora(dtpHoraInicio),
                horaFin = obtenerHora(dtpHoraFin),
                diaSemana = cbxDiaSemana.SelectedItem.ToString(),
                conductorPrincipal = txtConductor.Text.Trim()
            };

            try
            {
                if (esNuevo)
                {
                    HorarioProgramaCln.crear(programa);
                }
                else
                {
                    programa.id = obtenerIdSeleccionado();
                    HorarioProgramaCln.actualizar(programa);
                }
                listar();
                habilitarEdicion(false);
                MessageBox.Show("Programa guardado correctamente", TituloMensaje,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TituloError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            int id = obtenerIdSeleccionado();
            string titulo = dgvLista.CurrentRow.Cells["titulo"].Value.ToString();

            DialogResult dialog = MessageBox.Show($"¿Está seguro que desea dar de baja el programa {titulo}?",
                "::: Confirmación - Parcial 2 :::", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog != DialogResult.Yes) return;

            try
            {
                HorarioProgramaCln.eliminar(id);
                listar();
                MessageBox.Show("Programa dado de baja correctamente", TituloMensaje,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, TituloError, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

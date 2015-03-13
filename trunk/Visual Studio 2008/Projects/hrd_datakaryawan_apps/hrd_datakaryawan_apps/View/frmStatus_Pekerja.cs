﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using hrd_datakaryawan_apps.DAO;
using hrd_datakaryawan_apps.Model;

namespace hrd_datakaryawan_apps.View
{
    public partial class frmStatus_Pekerja : Form
    {
        //Variabel Global
        private frmMenuUtama frmMU = null;
        private Status_PekerjaDAO stPDAO = null;
        private bool resultBool = false;
        private int result = 0;

        //Constructor
        public frmStatus_Pekerja(DBConnection conn, frmMenuUtama frmMU)
        {
            InitializeComponent();

            try
            {
                stPDAO = new Status_PekerjaDAO(conn.GetConnection());

                loadDataStatusPekerja();

                this.frmMU = frmMU;
                frmMUEnbDis(false);

            }
            catch (Exception ex)
            {
                errorDBox(ex.Message.ToString(), "frmStatusPekerja_Constructor");
            }
        }

        private void statusStripStPPanel1(string strPanel1, System.Drawing.Color clr)
        {
            stPPanel1.Text = strPanel1;
            stPPanel1.ForeColor = clr;
        }

        private void errorDBox(string str1, string str2)
        {
            MessageBox.Show(str1 + "\n\n" + "Source : frmStatus_Pekerja\nMethod : " + str2);            
            statusStripStPPanel1("Telah terjadi Error.", Color.Red);

        }

        private void frmMUEnbDis(bool ft)
        {
            frmMU.statusPekerjaToolStripMenuItem.Enabled = ft;
        }

        private void loadDataStatusPekerja()
        {
            try
            {
                List<Status_Pekerja> daftarStP = stPDAO.GetAll();                
                DGV.DataSource = daftarStP.ToArray();
                DGV.ReadOnly = true;

                //Set otomatis lebar kolom
                DGV.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                DGV.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                DGV.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            catch (Exception ex)
            {
                errorDBox(ex.Message.ToString(), "loadDataStatusPekerja");
            }
        }
                
        private bool msgBoxWarning(string prompt)
        {
            if (MessageBox.Show(prompt, "Peringatan", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void clearTextBox()
        {
            txtKodeStatus.Clear();
            txtStatus.Clear();
            txtKodeStatus.Focus();
        }

        private void frmStatus_Pekerja_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (frmMU.CountAllActiveChildForm() == 1)
                {
                    //MessageBox.Show("Jumlah ChildForm yang aktif = " + frmMU.CountAllActiveChildForm().ToString());
                    frmMU.toolStripMenuItem1.Enabled = true;
                }

                frmMUEnbDis(true);
            }
            catch (Exception ex)
            {
                errorDBox(ex.Message.ToString(), "frmStatusPekerja_FormClosed");
            }
        }

        private void btnTutup_Click(object sender, EventArgs e)
        {
            try
            {
                frmMUEnbDis(true);
                this.Close();
            }
            catch (Exception ex)
            {
                errorDBox(ex.Message.ToString(), "btnTutup_Click");
            }
        }

        private void lblClearText_Click(object sender, EventArgs e)
        {
            clearTextBox();
            statusStripStPPanel1("Standby.", Color.Green);
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtKodeStatus.Text == "" || txtStatus.Text == "")
                {
                    statusStripStPPanel1("Field Kode_Status maupun Status tidak boleh kosong !", Color.Yellow);
                }
                else
                {
                    Status_Pekerja sTP = new Status_Pekerja();
                    sTP.Kode_Status = txtKodeStatus.Text;
                    sTP.Status = txtStatus.Text;

                    //Tentukan operasi Update atau Save
                    resultBool = stPDAO.cekRecord(sTP.Kode_Status);

                    if (resultBool == false) //Operasi Save
                    {
                        result = stPDAO.Save(sTP);

                        if (result > 0)
                        {
                            statusStripStPPanel1("Data berhasil disimpan.", Color.Green);
                            clearTextBox();

                            loadDataStatusPekerja();
                        }
                        else
                        {
                            statusStripStPPanel1("Data gagal disimpan.", Color.Red);
                        }
                    }
                    else //Operasi Update
                    {
                        if (msgBoxWarning("Anda yakin akan mengubah data Status Pekerja dengan Kode = " + sTP.Kode_Status.ToString() + " ?") == true)
                        //if (msgBoxWarning("Anda yakin akan mengubah data Status Pekerja dengan Kode = " + txtKodeStatus.Text + " ?") == true)
                        {
                            result = stPDAO.Update(sTP);

                            if (result > 0)
                            {
                                statusStripStPPanel1("Data berhasil diubah.", Color.Green);
                                clearTextBox();

                                loadDataStatusPekerja();
                            }
                            else
                            {
                                statusStripStPPanel1("Data gagal diubah.", Color.Red);
                            }
                        }
                        else
                        {
                            clearTextBox();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorDBox(ex.Message.ToString(), "btnSimpan_Click");
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            try
            {
                Status_Pekerja stP = new Status_Pekerja();
                stP.Kode_Status = txtKodeStatus.Text;

                if (txtKodeStatus.Text == "")
                {
                    statusStripStPPanel1("Tentukan Kode_Status yang akan dihapus !", Color.Yellow);
                }
                else
                {
                    if (msgBoxWarning("Apakah anda yakin akan menghapus data Status Pekerja dengan Kode \"" + stP.Kode_Status + "\" ?") == true)
                    {
                        result = stPDAO.Delete(stP.Kode_Status);

                        if (result > 0)
                        {
                            statusStripStPPanel1("Data berhasil dihapus.", Color.Green);
                        }
                        else
                        {
                            statusStripStPPanel1("Data gagal dihapus.", Color.Red);
                        }
                    }
                    else
                    {
                        clearTextBox();
                    }

                    loadDataStatusPekerja();
                }
            }
            catch (Exception ex)
            {
                errorDBox(ex.Message.ToString(), "btnHapus_Click");
            }
        }

        private void DGV_SelectionChanged(object sender, EventArgs e)
        {
            txtKodeStatus.Text = DGV.Rows[DGV.CurrentRow.Index].Cells[0].Value.ToString();
            txtStatus.Text = DGV.Rows[DGV.CurrentRow.Index].Cells[1].Value.ToString();
        }

        private void txtKodeStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                btnSimpan_Click(null, null);
            }       
        }

        private void txtStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                btnSimpan_Click(null, null);
            }
       
        }

        private void DGV_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                txtKodeStatus.Focus();
            }
       
        }
    }
}

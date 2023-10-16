using NHibernate;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WFormsNHibernate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            loadEmployeeData();
        }
        private int lastSelectEmployeeKey;

        private void loadEmployeeData()
        {
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                IQuery query = session.CreateQuery("FROM Employee");
                IList<Model.Employee> empInfo = query.List<Model.Employee>();

                dataGridView.DataSource = empInfo;
                dataGridView.Columns[0].Visible = false;
                textBoxId.Visible = false;
            }
        }

        private IList<Model.Employee> getDataFromEmployee(string id)
        {
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + id + "'");
                        return query.List<Model.Employee>();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }
        }



        private void SetEmployeInfo(Model.Employee emp)
        {

            emp.FirstName = textBoxFirstName.Text;
            emp.LastName = textBoxLastName.Text;
            emp.Email = textBoxEmail.Text;
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (textBoxFirstName.Text == "" ||
                textBoxLastName.Text == "" ||
                textBoxEmail.Text == "")
            {
                MessageBox.Show("Please fill in all values.");
                return;
            }

            Model.Employee empData = new Model.Employee();

            SetEmployeInfo(empData);

            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {


                        session.Save(empData);
                        transaction.Commit();
                        loadEmployeeData();
                        textBoxId.Visible = false;
                        textBoxFirstName.Text = "";
                        textBoxLastName.Text = "";
                        textBoxEmail.Text = "";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                        throw ex;
                    }
                }
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {

            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (lastSelectEmployeeKey == 0)
                        {
                            MessageBox.Show("Please select an employee first.");
                            return;
                        }


                        dataGridView.Columns[0].Visible = false;

                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + textBoxId.Text + "'");
                        Model.Employee empData = query.List<Model.Employee>()[0];
                        SetEmployeInfo(empData);
                        session.Update(empData);
                        transaction.Commit();

                        loadEmployeeData();


                        textBoxFirstName.Text = "";
                        textBoxLastName.Text = "";
                        textBoxEmail.Text = "";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.RowCount <= 1 || e.RowIndex < 0)
                return;
            string id = dataGridView[0, e.RowIndex].Value.ToString();

            if (id == "")
                return;

            IList<Model.Employee> empInfo = getDataFromEmployee(id);

            textBoxId.Text = empInfo[0].Id.ToString();

            textBoxId.Visible = false;
            textBoxFirstName.Text = empInfo[0].FirstName.ToString();
            textBoxLastName.Text = empInfo[0].LastName.ToString();
            textBoxEmail.Text = empInfo[0].Email.ToString();
            lastSelectEmployeeKey = empInfo[0].Id;
        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {
            loadEmployeeData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            ISession session = SessionFactory.OpenSession;

            using (session)
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    try
                    {
                        if (lastSelectEmployeeKey == 0)
                        {
                            MessageBox.Show("Please select an employee first.");
                            return;
                        }

                        IQuery query = session.CreateQuery("FROM Employee WHERE Id = '" + textBoxId.Text + "'");
                        Model.Employee empData = query.List<Model.Employee>()[0];
                        SetEmployeInfo(empData);
                        session.Delete(empData);
                        transaction.Commit();

                        loadEmployeeData();

                        textBoxId.Visible = false;
                        textBoxFirstName.Text = "";
                        textBoxLastName.Text = "";
                        textBoxEmail.Text = "";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}


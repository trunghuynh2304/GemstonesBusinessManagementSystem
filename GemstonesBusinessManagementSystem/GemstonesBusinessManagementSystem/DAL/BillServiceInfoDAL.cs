﻿using GemstonesBusinessManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemstonesBusinessManagementSystem.DAL
{
    class BillServiceInfoDAL : Connection
    {
        private static BillServiceInfoDAL instance;

        public static BillServiceInfoDAL Instance
        {
            get { if (instance == null) instance = new BillServiceInfoDAL(); return BillServiceInfoDAL.instance; }
            private set { BillServiceInfoDAL.instance = value; }
        }
        private BillServiceInfoDAL()
        {

        }

        public bool DeleteByIdBillService(string idBillService)
        {
            try
            {
                OpenConnection();
                string queryString = "DELETE FROM BillServiceInfo WHERE idBillService=" + idBillService;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool Delete(string idBillService, string idService)
        {
            try
            {
                OpenConnection();
                string queryString = "DELETE FROM BillServiceInfo WHERE idBillService=" + idBillService + " AND idService=" + idService;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                if (command.ExecuteNonQuery() < 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool Insert(BillServiceInfo billServiceInfo)
        {
            try
            {
                OpenConnection();
                string queryString = "INSERT INTO BillServiceInfo(idBillService, idService, quantity,paidMoney,status) VALUES(@idBillService, @idService, @quantity, @paidMoney, @status)";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBillService", billServiceInfo.IdBillService);
                command.Parameters.AddWithValue("@idService", billServiceInfo.IdService);
                command.Parameters.AddWithValue("@quantity", billServiceInfo.Quantity);
                command.Parameters.AddWithValue("@paidMoney", billServiceInfo.PaidMoney);
                command.Parameters.AddWithValue("@status", billServiceInfo.Status);
                //command.Parameters.AddWithValue("@deliveryDate", billServiceInfo.DeliveryDate);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool Update(BillServiceInfo billServiceInfo)
        {
            try
            {
                OpenConnection();
                string queryString = "UPDATE BillServiceInfo SET quantity=@quantity,paidMoney=@paidMoney,status=@status where idService=@idService and idBillService=@idBillService";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idService", billServiceInfo.IdService.ToString());
                command.Parameters.AddWithValue("@idBill", billServiceInfo.IdBillService.ToString());
                command.Parameters.AddWithValue("@quantity", billServiceInfo.Quantity.ToString());
                command.Parameters.AddWithValue("@paidMoney", billServiceInfo.PaidMoney);
                command.Parameters.AddWithValue("@status", billServiceInfo.Status);
                command.Parameters.AddWithValue("@deliveryDate", billServiceInfo.DeliveryDate);
                command.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
        public int CountSumMoney(string idBillService) // Tính tổng số tiền 
        {
            OpenConnection();
            int sum = 0;
            DataTable dataTable = new DataTable();
            string queryString = "SELECT BillServiceInfo.quantity,Price FROM BillServiceInfo Inner join Service on Service.idService = BillServiceInfo.idService where idBillService=@idBillService ";
            MySqlCommand commnad = new MySqlCommand(queryString, conn);
            commnad.Parameters.AddWithValue("@idBillService", idBillService);
            MySqlDataAdapter adapter = new MySqlDataAdapter(commnad);
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                sum += int.Parse(dataTable.Rows[i].ItemArray[0].ToString()) * int.Parse(dataTable.Rows[i].ItemArray[1].ToString());
            }
            CloseConnection();
            return sum;
        }
        public List<BillServiceInfo> GetBillServiceInfos(string idBillService)
        {
            List<BillServiceInfo> billInfos = new List<BillServiceInfo>();
            try
            {
                OpenConnection();
                string queryString = "SELECT * FROM BillServiceInfo WHERE idBillService=" + idBillService;
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i].ItemArray[0].ToString() == idBillService)
                    {
                        BillServiceInfo billInfo = new BillServiceInfo(int.Parse(dataTable.Rows[i].ItemArray[0].ToString()), 
                            int.Parse(dataTable.Rows[i].ItemArray[1].ToString()), int.Parse(dataTable.Rows[i].ItemArray[2].ToString()), 
                            float.Parse(dataTable.Rows[i].ItemArray[3].ToString()), int.Parse(dataTable.Rows[i].ItemArray[4].ToString()), 
                            DateTime.Parse(dataTable.Rows[i].ItemArray[5].ToString()));
                        billInfos.Add(billInfo);
                    }
                }
                return billInfos;
            }
            catch
            {
                return billInfos;
            }
            finally
            {
                CloseConnection();
            }
        }
        public bool IsExisted(string idBillService, string idService)
        {
            try
            {
                OpenConnection();
                string queryString = "SELECT * FROM BillServiceInfo WHERE idBillService=@idBillService AND idService=@idService;";
                MySqlCommand command = new MySqlCommand(queryString, conn);
                command.Parameters.AddWithValue("@idBillService", idBillService);
                command.Parameters.AddWithValue("@idService", idService);
                int rs = command.ExecuteNonQuery();
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable.Rows.Count == 1;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}

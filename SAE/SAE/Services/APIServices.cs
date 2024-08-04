using Newtonsoft.Json;
using SAE.Models.Inventory;
using SAE.Models.Sale;
using System.Net.Http.Headers;
using System.Text;

namespace SAE.Services
{
    public class APIServices
    {
        #region Variables

        HttpClient _client; 

        #endregion

        #region Constructors

        public APIServices()
        {
            _client = new HttpClient();
        } 

        #endregion

        #region Items

        public async Task<List<ItemModel>> GetItems()
        {
            List<ItemModel> items = new List<ItemModel>();

            Uri uri = new Uri(string.Format(Constans.URL + "Items", string.Empty));
            try
            {
                string _UserName = "11162181";
                string _Password = "60-dayfreetrial";
                var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemModel>>(content);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return items;
        }

        public async Task<ItemModel> SaveItemModelAsync(ItemModel item)
        {
            Uri uri = new Uri(string.Format(Constans.URL + "Items", string.Empty));

            try
            {
                string _UserName = "11162181";
                string _Password = "60-dayfreetrial";
                var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                string json =  JsonConvert.SerializeObject(item);
                StringContent contents = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (item.Id == 0)
                    response = await _client.PostAsync(uri, contents);
                else
                    response = await _client.PutAsync(uri, contents);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    item = JsonConvert.DeserializeObject<ItemModel>(content);
                }
                else
                {
                    throw new Exception("Ha ocurrido un error al actualizar: " + response.RequestMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return item;
        }

        public async Task<bool> DeleteItem(int id)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            Uri uri = new Uri(string.Format(Constans.URL + $"Items/{id}", string.Empty));

            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(uri);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Ha ocurrido un error al realizar la petición: {response.RequestMessage}");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        #endregion

        #region DeliveryNotes

        public async Task<List<ItemDeliveryNoteModel>> GetItemsDeliveryNote(int deliverynoteid)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            List<ItemDeliveryNoteModel> items = new List<ItemDeliveryNoteModel>();

            Uri uri = new Uri(string.Format(Constans.URL + "ItemDeliveryNote/"+ deliverynoteid, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemDeliveryNoteModel>>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return items;
        }

        public async Task<DeliveryNoteModel> SaveDeliveryNotelAsync(DeliveryNoteModel dNote)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            Uri uri = new Uri(string.Format(Constans.URL + "DeliveryNotes", string.Empty));

            try
            {
                string json =  JsonConvert.SerializeObject(dNote);
                //string json =  JsonSerializer.Serialize<ItemModel>(item, _serializerOptions);
                StringContent contents = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (dNote.Id == 0)
                response = await _client.PostAsync(uri, contents);
                else
                response = await _client.PutAsync(uri, contents);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    dNote = JsonConvert.DeserializeObject<DeliveryNoteModel>(content);
                }
                else
                {
                    throw new Exception("Ha ocurrido un error al actualizar: " + response.RequestMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dNote;
        }

        public async Task<List<DeliveryNoteModel>> GetDeliveryNotes()
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            List<DeliveryNoteModel> dNotes = new List<DeliveryNoteModel>();

            Uri uri = new Uri(string.Format(Constans.URL + "DeliveryNotes", string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    dNotes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DeliveryNoteModel>>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dNotes;
        }

        public async Task<bool> DeleteDeliveryNote(int id)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            Uri uri = new Uri(string.Format(Constans.URL + $"DeliveryNotes/{id}", string.Empty));

            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(uri);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Ha ocurrido un error al realizar la petición: {response.RequestMessage}");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        #endregion

        #region Debtors

        public async Task<List<DebtorModel>> GetDebtors()
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            List<DebtorModel> dNotes = new List<DebtorModel>();

            Uri uri = new Uri(string.Format(Constans.URL + "Debtor", string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    dNotes = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DebtorModel>>(content);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dNotes;
        }

        public async Task<List<DebtorPaymentModel>> GetPayments(int debtorId)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            List<DebtorPaymentModel> payments = new List<DebtorPaymentModel>();

            Uri uri = new Uri(string.Format(Constans.URL + "Debtor/"+ debtorId, string.Empty));
            try
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    DebtorModel debtor = Newtonsoft.Json.JsonConvert.DeserializeObject<DebtorModel>(content);
                    if (debtor != null && debtor.Payments != null && debtor.Payments.Count > 0)
                        payments = debtor.Payments;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return payments;
        }

        public async Task<DebtorModel> SaveDebtorAsync(DebtorModel debtor)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            Uri uri = new Uri(string.Format(Constans.URL + "Debtor", string.Empty));

            try
            {
                string json =  JsonConvert.SerializeObject(debtor);
                StringContent contents = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null; 
                if (debtor.Id == 0)
                response = await _client.PostAsync(uri, contents);
                else
                response = await _client.PutAsync(uri, contents);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    debtor = JsonConvert.DeserializeObject<DebtorModel>(content);
                }
                else
                    throw new Exception("Ha ocurrido un error al actualizar: " + response.RequestMessage.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return debtor;
        }

        public async Task<bool> DeleteDebtorAsync(int id)
        {
            string _UserName = "11162181";
            string _Password = "60-dayfreetrial";
            var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
            Uri uri = new Uri(string.Format(Constans.URL + $"Debtor/{id}", string.Empty));

            try
            {
                HttpResponseMessage response = await _client.DeleteAsync(uri);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Ha ocurrido un error al realizar la petición: {response.RequestMessage}");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        #endregion

        #region Customer

        public async Task<List<CustomerModel>> GetCustomers()
        {
            List<CustomerModel> customers = new List<CustomerModel>();

            Uri uri = new Uri(string.Format(Constans.URL + "Customer", string.Empty));
            try
            {
                string _UserName = "11162181";
                string _Password = "60-dayfreetrial";
                var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                HttpResponseMessage response = await _client.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    customers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CustomerModel>>(content);
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return customers;
        }

        public async Task<CustomerModel> SaveCustomerModelAsync(CustomerModel customer)
        {
            Uri uri = new Uri(string.Format(Constans.URL + "Customer", string.Empty));

            try
            {
                string _UserName = "11162181";
                string _Password = "60-dayfreetrial";
                var authToken = Encoding.ASCII.GetBytes(_UserName + ":" + _Password);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

                string json =  JsonConvert.SerializeObject(customer);
                //string json =  JsonSerializer.Serialize<CustomerModel>(Customer, _serializerOptions);
                StringContent contents = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (customer.Id == 0)
                    response = await _client.PostAsync(uri, contents);
                else
                    response = await _client.PutAsync(uri, contents);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    customer = JsonConvert.DeserializeObject<CustomerModel>(content);
                }
                else
                {
                    throw new Exception("Ha ocurrido un error al actualizar: " + response.RequestMessage.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return customer;
        }

        #endregion
    }
}


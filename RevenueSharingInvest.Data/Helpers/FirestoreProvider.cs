using Google.Cloud.Firestore;
using Google.Type;
using iText.StyledXmlParser.Jsoup.Helper;
using RevenueSharingInvest.Data.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Helpers
{
    public class FirestoreProvider
    {
        private readonly FirestoreDb _fireStoreDb;

        public FirestoreProvider(FirestoreDb fireStoreDb)
        {
            _fireStoreDb = fireStoreDb;
        }

        public async Task<List<BillEntity>> CreateBills(List<BillEntity> billList, string projectId)
        {
            try
            {
                foreach(BillEntity bill in billList)
                {
                    DocumentReference billPath = _fireStoreDb.Collection("Bills").Document(projectId).Collection(DateTimePicker.GetDateTimeByTimeZone().ToString("dd-MM-yyyy")).Document(bill.InvoiceId);
                    await billPath.SetAsync(bill);
                }

                return billList;
            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<dynamic> UpdateBills(BillEntity bill, string date)
        {
            try
            {
                DocumentReference billPath = _fireStoreDb.Collection("Bills").Document(bill.ProjectId).Collection(date).Document(bill.InvoiceId);
                return await billPath.SetAsync(bill);
            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }



        public async Task<List<string>> GetDatesOfProject(string projectId)
        {
            try
            {
                List<string> dateList = new();
                DocumentReference docRef = _fireStoreDb.Collection("Bills").Document("ProjectId");
                DocumentSnapshot documentSnapshot = await docRef.GetSnapshotAsync();

                IAsyncEnumerable<CollectionReference> subcollections = docRef.ListCollectionsAsync();
                IAsyncEnumerator<CollectionReference> subcollectionsEnumerator = subcollections.GetAsyncEnumerator(default);
                while (await subcollectionsEnumerator.MoveNextAsync())
                {
                    CollectionReference subcollectionRef = subcollectionsEnumerator.Current;
                    dateList.Add(subcollectionRef.Id);
                }

                return dateList;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<List<string>> GetProjectInvoiceIdByDate(string projectId, string date)
        {
            try
            {
                List<string> invoiceList = new();
                Query query = _fireStoreDb.Collection("Bills").Document("ProjectId").Collection(date);
                QuerySnapshot allInvoice = await query.GetSnapshotAsync();
                foreach (DocumentSnapshot documentSnapshot in allInvoice.Documents)
                {
                    invoiceList.Add(documentSnapshot.Id);
                }
                return invoiceList;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public async Task<List<BillEntity>> GetInvoiceDetailByDate(string projectId, string date)
        {
            try
            {
                List<BillEntity> bills = new();
                List<string> invoiceIdList = await GetProjectInvoiceIdByDate(projectId, date);
                foreach (string invoiceId in invoiceIdList)
                {
                    DocumentReference documentReference = _fireStoreDb.Collection("Bills").Document(projectId).Collection(date).Document(invoiceId);
                    DocumentSnapshot documentSnapshot = await documentReference.GetSnapshotAsync();
                    BillEntity bill = documentSnapshot.ConvertTo<BillEntity>();

                    bills.Add(bill);
                }
                return bills;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }





        [FirestoreData]
        public class BillEntity
        {
            [FirestoreProperty]
            public string BillId { get; set; }
            [FirestoreProperty]
            public string InvoiceId { get; set; }
            [FirestoreProperty]
            public string Descriprion { get; set; }
            [FirestoreProperty]
            public string Createdate { get; set; }
            [FirestoreProperty]
            public string Createby { get; set; }
            [FirestoreProperty]
            public string Preference { get; set; }
            [FirestoreProperty]
            public string Amount { get; set; }
            [FirestoreProperty]
            public string ProjectId { get; set; }
        }
    }
}

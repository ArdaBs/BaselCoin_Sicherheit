using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace NoSQLSkiServiceManager.DTOs.Request
{
    public class AccountBalanceUpdateDto
    {

        [Range(0, double.MaxValue)]
        public double NewBalance { get; set; }
    }

}

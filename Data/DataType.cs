using SwaggerTest.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwaggerTest.Data
{
    public class DataTypeTable
    {
        [Key]
        public Guid Id { get; set; }
        public byte Byte { get; set; }
        public sbyte SByte { get; set; }        
        public ushort USHort { get; set; }
        public short Short { get; set; }
        public uint UInt { get; set; }
        public int Int { get; set; }
        public ulong ULong { get; set; }
        public long Long { get; set; }
        public float Float { get; set; }
        public double Double { get; set; }
        [DataType(DataType.Currency)]
        public double Currency { get; set; }

        public decimal Decimal { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Text)]
        public string Text { get; set; }

        // nvarchar
        public string MaxNvarchar { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string LimitNvarchar { get; set; }

        public GenderEnum GenderEnum { get; set; }
        public GenderByteEnum GenderByteEnum { get; set; }
    }
}

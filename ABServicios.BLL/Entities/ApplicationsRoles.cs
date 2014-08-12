namespace ABServicios.BLL.Entities
{
	public static class ApplicationsRoles
	{
		public const string Admin = "Admin";
        public const string Transporte = "Transporte";
        public const string DolarBlue = "DolarBlue";
        public static string[] InHouseApplication = new string[] { Admin, Transporte, DolarBlue };
	}
}
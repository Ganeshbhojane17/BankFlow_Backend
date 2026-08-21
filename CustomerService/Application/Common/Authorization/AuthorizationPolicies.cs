namespace CustomerService.Application.Common.Authorization
{
    public static class AuthorizationPolicies
    {
        public const string ViewCustomers =
            "ViewCustomers";

        public const string ManageCustomers =
            "ManageCustomers";

        public const string DeleteCustomers =
            "DeleteCustomers";

        public const string ChangeCustomerStatus =
            "ChangeCustomerStatus";
    }
}

interface ICustomerContactRequest {
  Id:number;
  IsPrimary:boolean;
  CustomerId:number;
  ContactId:number;
  Contact:IContactRequest;
}

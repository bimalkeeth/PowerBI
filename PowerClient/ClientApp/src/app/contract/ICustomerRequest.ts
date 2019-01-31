export interface ICustomerRequest {
  Id:number;
  CustomerCode:string;
  FirstName:string;
  LastName:string;
  DateOfBirth:string;
  CustomerContacts:ICustomerContactRequest[];
  CustomerAddress:ICustomerAddressRequest[];
}

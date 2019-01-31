interface ICustomerAddressRequest {
  Id:number;
  AddressId:number;
  CustomerId:number;
  IsPrimary:boolean;
  Address:IAddressRequest;
}

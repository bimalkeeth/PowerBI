import {Component, Inject, OnInit, OnDestroy} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators} from "@angular/forms";
import {ToastrService} from "ngx-toastr";
import "rxjs-compat/add/operator/map";
import {MoulaCustomer} from "../contract/MoulaCustomer";
import 'ngx-toastr/toastr.css';
import {ValidationUtil} from "../utility/ValidationUtil";
import {CustomerServiceService} from "../services/customer-service.service";
import {map} from "rxjs/operators";
import {ICustomerDetail} from "../contract/ICustomerDetail";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls:['./home.component.css']
})
export class HomeComponent implements OnInit, OnDestroy{

  moulaCustomer:MoulaCustomer;
  frmMoulaFormGroup: FormGroup;
  inputIdControl: FormControl;
  inputFirstNameControl:FormControl;
  inputLastNameControl:FormControl;
  inputEmailControl:FormControl;
  inputDateOfBirthControl:FormControl;
  topListOfCustomer:any[];

  constructor( @Inject(CustomerServiceService) public customerService: CustomerServiceService,
               @Inject(FormBuilder) public formBuilder: FormBuilder,
               @Inject(ToastrService) public toastr: ToastrService
               )
  {
    this.moulaCustomer=new MoulaCustomer();
  }

  ngOnDestroy(): void {

  }

  ngOnInit(): void {

    this.inputIdControl = new FormControl(this.moulaCustomer.Id,{updateOn: 'blur'});
    this.inputFirstNameControl = new FormControl(this.moulaCustomer.FirstName,{ validators: Validators.required,updateOn: 'blur'});
    this.inputLastNameControl = new FormControl(this.moulaCustomer.LastName,{ validators: Validators.required,updateOn: 'blur'});
    this.inputEmailControl = new FormControl(this.moulaCustomer.EmailAddress,{ validators: [Validators.required,Validators.email],updateOn: 'blur'});
    this.inputDateOfBirthControl = new FormControl(this.moulaCustomer.DateOfBirth,{ validators: Validators.required,updateOn: 'blur'});

    this.frmMoulaFormGroup = new FormGroup({
      customerIdCtrl: this.inputIdControl,
      customerFirstNameCtrl:this.inputFirstNameControl,
      customerLastNameCtrl:this.inputLastNameControl,
      customerEmailCtrl:this.inputEmailControl,
      customerDateOfBirthCtrl:this.inputDateOfBirthControl,
    });
    this.LoadCustomersTop();

  }

  LoadCustomersTop(){
    this.customerService.GetTopCustomers(3).toPromise().then(data => {
      this.topListOfCustomer = data;

    });
  }

  ProcessData():void{
    try {
          if(!ValidationUtil.IsEmail(this.moulaCustomer.EmailAddress)){
            this.toastr.error('Email Address is not correct.', 'Major Error', {
              timeOut: 3000
            });
            return;
          }
          if(!ValidationUtil.IsNotEmpty(this.moulaCustomer.DateOfBirth)){
            this.toastr.error('Date Of Birth is not correct.', 'Major Error', {
              timeOut: 3000
            });
            return;
          }
          if(!ValidationUtil.IsNotEmpty(this.moulaCustomer.FirstName)){
            this.toastr.error('First Name Can not be empty.', 'Major Error', {
              timeOut: 3000
            });
            return;
          }
          if(!ValidationUtil.IsNotEmpty(this.moulaCustomer.LastName)){
            this.toastr.error('Last Name Can not be empty.', 'Major Error', {
              timeOut: 3000
            });
            return;
          }

          let customerContacts={
            Id:0,
            IsPrimary:true,
            CustomerId:0,
            ContactId:0,
            Contact:{Id:0,ContactTypeId:1,Contact:this.moulaCustomer.EmailAddress}
          };

          let customerAddress={
            Id:0,
            IsPrimary:true,
            CustomerId:0,
            AddressId:0,
            Address:{Id:0,AddressTypeId:2,Street:'2-5',Street2:'Wattle Road',Suburb:'Maidstone',StateId:2,Country:'Australia'}
          };
          let customerRequest={
            Id:0,
            CustomerCode:'',
            FirstName:this.moulaCustomer.FirstName,
            LastName:this.moulaCustomer.LastName,
            DateOfBirth:this.moulaCustomer.DateOfBirth,
            CustomerContacts:[customerContacts],
            CustomerAddress:[customerAddress]
          };
          this.customerService.CreateCustomer(customerRequest);
          this.LoadCustomersTop();
    }
    catch(e){
      this.toastr.error('Error occured when creating customer.', 'Major Error', {
        timeOut: 3000
      });
    }
  }

}

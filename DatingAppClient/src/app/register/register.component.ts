import { Component, OnInit,Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  model: any = {}
  constructor(private accountService:AccountService, private toast:ToastrService) {}

  ngOnInit(): void {
  }

  register(){
    this.accountService.register(this.model).subscribe({
      next: () => {
        
        this.cancel();
      },
      error: error => {
        this.toast.error(error.error.title)
        
      }
    })
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

}

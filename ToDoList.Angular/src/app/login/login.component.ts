import { Component, inject } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms'
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { RouterLink, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  providers:[AuthService]
})
export class LoginComponent {
  form!: FormGroup

  service = inject(AuthService);

  toastr = inject(ToastrService);

  router= inject(Router);
  


  constructor(public formBuilder: FormBuilder){
    
    this.form = this.formBuilder.group({
      email: [''],
      password: ['']
    })
  }
  

  onSubmit(){
      this.service.loginUser(this.form.value)
      .subscribe({
        next:(res:any)=>{
            
            const token = res.token;
            sessionStorage.setItem('authToken', token); 
            this.form.reset();
            this.toastr.success("Login Successful");
            this.router.navigateByUrl('/todos');

        },
        error:err=>{
          console.log(err.error);
          this.toastr.error(`${err.error}`,"Please Tray0 Again")
        }
      })
  }
  
}

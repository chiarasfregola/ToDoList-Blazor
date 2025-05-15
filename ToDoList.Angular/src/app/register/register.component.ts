import { Component, inject } from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms'
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
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
     
      this.service.createUser(this.form.value)
      .subscribe({
        next:res=>{
            this.form.reset();
            this.toastr.success("Registrazione avvenuta con successo");
            this.router.navigateByUrl('/')

        },
        error:(err:any)=>{
      

          if (typeof err.error === "string") {
            this.toastr.error(err.error, "Validation Error");
          } 
          else if (Array.isArray(err.error)) {
            err.error.forEach((e: any) => {
              this.toastr.error(e.description, "Validation Error");
            });
          } 
          else if (err.error && err.error.errors) {
            Object.entries(err.error.errors).forEach(([key, messages]) => {
              (messages as string[]).forEach((message: string) => {
                this.toastr.error(message, "Validation Error");
              });
            });
          } 
          else {
            this.toastr.error("Error");
          }
        }
      })
  }
  goBack(){
    this.router.navigateByUrl('/')
  }
}

import { HttpClient } from '@angular/common/http';
import { inject, Injectable} from '@angular/core';
import { Router } from '@angular/router';



@Injectable({
  providedIn: 'root'
})
export class AuthService {

  router=inject(Router);
  http=inject(HttpClient);
  baseUrl = 'https://localhost:7152';
  private tokenKey = 'authToken';

  createUser(formData:any){
    const Url = this.baseUrl+'/api/Register';
    return this.http.post(Url, formData)
  }

  loginUser(formData:any){
    const Url = this.baseUrl+'/api/Login';
    return this.http.post(Url, formData)
  }

  // Metodo per salvare il token
  saveToken(token: string): void {
    sessionStorage.setItem(this.tokenKey, token); 
  }

  // Metodo per recuperare il token
  getToken(): string | null {
      return sessionStorage.getItem(this.tokenKey);
    
  }
  
  // Metodo per rimuovere il token (logout)
  logoutUser(): void {
    sessionStorage.removeItem(this.tokenKey);
    this.router.navigateByUrl('/')
  }
}


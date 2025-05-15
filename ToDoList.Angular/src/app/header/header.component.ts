import { Component, inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { Router} from '@angular/router';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  authService = inject(AuthService);
  router = inject(Router);

  isAuthenticated(): boolean {
    return this.authService.getToken() !== null;
  }

    logout(): void {
    const confirmation = window.confirm('Vuoi davvero uscire dalla tua ToDo List?');
    if(confirmation){
      this.authService.logoutUser();
      this.router.navigate(['/']); 
    }
  }
}

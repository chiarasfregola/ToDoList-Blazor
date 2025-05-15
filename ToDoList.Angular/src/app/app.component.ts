import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './header/header.component';


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  template:`
    
    <app-login></app-login>
    <app-register></app-register>
    <app-todos></app-todos>
    <main>
    </main>
    <router-outlet/>
    `,
})
export class AppComponent {
  Title = 'todo-app';
}

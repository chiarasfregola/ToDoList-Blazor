import { Routes } from '@angular/router';

export const routes: Routes = [
    
    {
        path: '',
        pathMatch: 'full',
        loadComponent:()=>{
            return import('./login/login.component').then(
                m=>m.LoginComponent
                )
        }
    },
    
    {
        path: 'register',
        loadComponent:()=>{
            return import('./register/register.component').then(
                m=>m.RegisterComponent
                )
        }
    },
    {
        path: 'todos',
        loadComponent:()=>{
            return import('./todos/todos.component').then(
                m=>m.TodosComponent
                )
        }
    }
];

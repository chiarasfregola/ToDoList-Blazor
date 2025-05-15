import { Component, inject, OnInit, signal, HostListener } from '@angular/core';
import { TodosService } from '../services/todos.service';
import { CommonModule } from '@angular/common'; 
import { todoItem } from '../models/todo.types';
import { catchError } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { HighlightCompletedTodoDirective } from '../directives/highlight-completed-todo.directive';
import { AuthService } from '../services/auth.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-todos',
  standalone: true, 
  imports: [CommonModule, FormsModule], 
  templateUrl: './todos.component.html',
  styleUrls: ['./todos.component.css']
})
export class TodosComponent implements OnInit {
  todoService = inject(TodosService);
  authService = inject(AuthService);
  toastr = inject(ToastrService);
  route = inject(Router);

  todoItems = signal<Array<todoItem>>([]);
  newTodoTitle: string = '';

  editTodoId: number | null = null;
  editTodoTitle: string = '';

  ngOnInit(): void {
    const token = this.authService.getToken();
    
    if(token != null){
      this.todoService.getUserTodos()
        .pipe(
          catchError((err) => {
            console.log(err);
            throw err;
          })
        )
        .subscribe((todos) => {
          this.todoItems.set(todos);
        });
    } else {
      this.toastr.error('You are not logged in: please go to login or register first.');
      this.route.navigateByUrl("/");
    }
  }

  get sortedTodoItems(): todoItem[] {
    return this.todoItems().slice().sort((a, b) => Number(a.isDone) - Number(b.isDone));
  }

  addTodo(): void {
    if (!this.newTodoTitle.trim()) return;

    let newTodo: todoItem = {
      title: this.newTodoTitle,
      isDone: false,
      id: 0,
      userId: '',
    };

    this.todoService.addTodoTitle(newTodo)
      .pipe(
        catchError((err) => {
          console.log(err);
          throw err;
        })
      )
      .subscribe((addedTodo) => {
        this.todoItems.set([...this.todoItems(), addedTodo]);
        this.newTodoTitle = '';
      });
  }

  deleteTodo(todoId: number): void {
    if(window.confirm('Sei sicuro di voler eliminare questo elemento?')){
      this.todoService.deleteTodoItem(todoId).subscribe(() => {
        this.todoItems.set(this.todoItems().filter(t => t.id !== todoId));
      });
    }
  }

  updateToDo(todo: todoItem) {
    const updatedTodo = {
      ...todo,
      isDone: !todo.isDone
    };

    this.todoService.updateToDoItem(updatedTodo).subscribe({
      next: () => {
        this.todoItems.set(
          this.todoItems().map(t => t.id === todo.id ? {...t, isDone: updatedTodo.isDone} : t)
        );
      },
      error: (err) => {
        console.error('Update error', err);
      }
    });
  }

  editTodoTitleFn(todo: todoItem) {
    this.editTodoId = todo.id;
    this.editTodoTitle = todo.title;
  }

  cancelEdit() {
    this.editTodoId = null;
    this.editTodoTitle = '';
  }

  saveTodoTitle(todo: todoItem) {
    if (!this.editTodoTitle.trim()) {
      alert('Please enter a valid activity');
      return;
    }

    const updatedTodo = {...todo, title: this.editTodoTitle};

    this.todoService.updateToDoItem(updatedTodo).subscribe({
      next: () => {
        this.todoItems.set(
          this.todoItems().map(t => t.id === todo.id ? {...t, title: this.editTodoTitle} : t)
        );
        this.editTodoId = null;
        this.editTodoTitle = '';
      },
      error: (err) => {
        console.error('Update error :', err);
      }
    });
  }

  @HostListener('document:keydown.escape', ['$event'])
  onEscPressed(event: KeyboardEvent) {
    if(this.editTodoId !== null) {
      this.cancelEdit();
      event.preventDefault();
    }
  }
}
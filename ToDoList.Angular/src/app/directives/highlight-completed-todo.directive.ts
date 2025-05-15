import { Directive, Input, OnChanges, SimpleChanges, ElementRef, inject } from '@angular/core';

@Directive({
  selector: '[appHighlightCompletedTodo]'
})
export class HighlightCompletedTodoDirective implements OnChanges {
  @Input() isCompleted: boolean = false; // Input che riceve il valore isDone dal componente

  private el = inject(ElementRef);

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isCompleted']) {
      this.updateStyles();
    }
  }
  
  private updateStyles(): void {
    const nativeElement = this.el.nativeElement;
    // Se isCompleted è true, applica gli stili
    if (this.isCompleted) {
      nativeElement.style.backgroundColor = '#d4edda'; // Verde chiaro
      nativeElement.style.color = '#155724'; // Colore verde scuro
      nativeElement.style.textDecoration = 'line-through'; // Barrato
    } else {
      // Altrimenti rimuovi gli stili
      nativeElement.style.backgroundColor = '';
      nativeElement.style.color = '';
      nativeElement.style.textDecoration = ''; // Rimuovi barrato
    }
  }
}


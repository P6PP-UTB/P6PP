import { Component } from '@angular/core';
import { HeaderComponent } from "../components/header/header.component";
import { NavigationComponent } from "../components/navigation/navigation.component";

@Component({
  selector: 'app-payment',
  imports: [NavigationComponent],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss'
})
export class PaymentComponent {

  paymentAmount = "0,0CZK";
  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
  
    const rawValue = input.value.replace(/[^\d]/g, '');
  

    const newValue = rawValue ? `${rawValue} CZK` : '';
  
    this.paymentAmount = newValue || '0,0CZK';
  
   
    input.value = newValue;
  
    const cursorPosition = rawValue.length;
    input.setSelectionRange(cursorPosition, cursorPosition);
  }
  
  
  
}

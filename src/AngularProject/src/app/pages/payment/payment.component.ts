import { Component } from '@angular/core';
import { NavigationComponent } from "../../components/navigation/navigation.component";

@Component({
  selector: 'app-payment',
  imports: [NavigationComponent],
  templateUrl: './payment.component.html',
  styleUrl: './payment.component.scss'
})
export class PaymentComponent {
  paymentAmount = '0,0CZK';
  private numericAmount = 0;

  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const rawValue = input.value.replace(/[^\d]/g, '');
    this.numericAmount = rawValue ? parseInt(rawValue, 10) : 0;

    const newValue = this.numericAmount > 0 ? `${this.numericAmount} CZK` : '0,0CZK';
    this.paymentAmount = newValue;
  }

  setFixedAmount(amount: number): void {
    this.numericAmount = amount;
    this.paymentAmount = `${this.numericAmount} CZK`;

    const input = document.getElementById('amountInput') as HTMLInputElement;
    if (input) {
      input.value = this.paymentAmount;
      input.focus();
      input.setSelectionRange(this.paymentAmount.length, this.paymentAmount.length);
    }
  }
}


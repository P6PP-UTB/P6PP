import { Component,Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { MatIconModule } from '@angular/material/icon';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-sidebar-menu',
  standalone: true,
  imports: [CommonModule,MatIconModule],
  templateUrl: './sidebar-menu.component.html',
  styleUrl: './sidebar-menu.component.scss',
  animations: [
    trigger('menuAnimation', [
      state('closed', style({
        transform: 'translateX(100%)', 
        opacity: 0
      })),
      state('open', style({
        transform: 'translateX(0)',
        opacity: 1
      })),
      transition('closed => open', [
        animate('300ms ease-in-out') 
      ]),
      transition('open => closed', [
        animate('200ms ease-in-out') 
      ])
    ])
  ]
})
export class SidebarMenuComponent {
  @Input() isOpen = false;
  @Output() menuClosed = new EventEmitter<void>();

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService 
  ) {}
  
  closeMenu(){
    this.isOpen = false;
    this.menuClosed.emit();
  }

  logout() {
    this.authService.logout();
    this.toastr.success('You have successfully logged out', 'Goodbye!');
    this.closeMenu();
  }
}

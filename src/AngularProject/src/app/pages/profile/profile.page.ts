import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { catchError, finalize, of } from 'rxjs';

@Component({
  selector: 'app-profile.page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavigationComponent, FooterComponent],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.scss'
})
export class ProfilePage implements OnInit {
  user: any = null;
  showEditForm = false;
  showSettingsForm = false;
  editForm: FormGroup;
  settingsForm: FormGroup;
  loading = true;
  error: string | null = null;

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    private toastr: ToastrService,
    private router: Router
  ) {
    // Initialize forms with empty values initially
    this.editForm = this.fb.group({
      username: [''],
      firstName: [''],
      lastName: [''],
      email: [''],
      phoneNumber: [''],
      weight: [''],
      height: [''],
      sex: ['']
    });

    this.settingsForm = this.fb.group({
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$')
      ]]
    });
  }

  ngOnInit() {
    this.loading = true;
    this.error = null;
    
    this.userService.getCurrentUser()
      .pipe(
        catchError(error => {
          console.error('Error fetching profile:', error);
          this.error = 'Failed to load profile data. Please try again later.';
          
          // Check if it's an auth error
          if (error.status === 401) {
            this.toastr.error('Your session has expired. Please log in again.');
            localStorage.removeItem('token');
            this.router.navigate(['/login']);
          }
          
          return of(null);
        }),
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe(response => {
        console.log('User response:', response);
        
        if (response && response.data && response.data.user) {
          this.user = response.data.user;
          this.updateForms();
        } else {
          this.error = 'Could not retrieve user data';
          console.error('User data not available in response', response);
        }
      });
  }

  updateForms() {
    if (!this.user) return;
    
    // Update the edit form with user data
    this.editForm.patchValue({
      username: this.user.username || '',
      firstName: this.user.firstName || '',
      lastName: this.user.lastName || '',
      email: this.user.email || '',
      phoneNumber: this.user.phoneNumber || '',
      weight: this.user.weight || '',
      height: this.user.height || '',
      sex: this.user.sex || ''
    });
  }

  toggleEditForm() {
    this.showEditForm = !this.showEditForm;
    if (this.showEditForm) {
      this.showSettingsForm = false;
      // Ensure form is updated with latest user data
      this.updateForms();
    }
  }

  toggleSettingsForm() {
    this.showSettingsForm = !this.showSettingsForm;
    if (this.showSettingsForm) {
      this.showEditForm = false;
      // Reset password field when showing the form
      this.settingsForm.get('newPassword')?.reset('');
    }
  }

  onSaveProfile() {
    if (!this.user || !this.user.id) {
      this.toastr.error('User information is not available');
      return;
    }

    if (!this.editForm.valid) {
      this.toastr.error('Please correct the form errors before submitting');
      return;
    }

    const raw = this.editForm.value;
    const cleaned: any = {};
    
    // Filter out empty values
    for (const key in raw) {
      if (raw[key] !== null && raw[key] !== '' && raw[key].toString().trim() !== '') {
        cleaned[key] = raw[key];
      }
    }
    
    if (Object.keys(cleaned).length === 0) {
      this.toastr.warning('No changes to save');
      return;
    }

    this.userService.updateUser(this.user.id, cleaned)
      .pipe(
        catchError(error => {
          console.error('Error updating profile:', error);
          this.toastr.error('Failed to update profile: ' + (error.message || 'Unknown error'));
          return of(null);
        })
      )
      .subscribe(response => {
        if (response) {
          this.showEditForm = false;
          this.toastr.success('Profile updated successfully!');
          // Refresh user data
          this.ngOnInit();
        }
      });
  }

  onResetPassword() {
    if (!this.user || !this.user.id) {
      this.toastr.error('User information is not available');
      return;
    }

    if (this.settingsForm.invalid) {
      this.toastr.error('Password does not meet requirements');
      return;
    }

    const newPassword = this.settingsForm.get('newPassword')?.value;
    if (!newPassword) {
      this.toastr.error('New password is required');
      return;
    }

    this.userService.resetPassword(this.user.id, newPassword)
      .pipe(
        catchError(error => {
          console.error('Error resetting password:', error);
          this.toastr.error('Failed to reset password: ' + (error.message || 'Unknown error'));
          return of(null);
        })
      )
      .subscribe(response => {
        if (response) {
          this.showSettingsForm = false;
          this.toastr.success('Password reset successfully!');
        }
      });
  }

  // onDeleteAccount() {
  //   if (!this.user || !this.user.id) {
  //     this.toastr.error('User information is not available');
  //     return;
  //   }
  //
  //   const confirmDelete = window.confirm('Are you sure you want to delete your account? This action is irreversible.');
  //   if (confirmDelete) {
  //     this.userService.deleteUser(this.user.id)
  //       .pipe(
  //         catchError(error => {
  //           console.error('Error deleting account:', error);
  //           this.toastr.error('Failed to delete account: ' + (error.message || 'Unknown error'));
  //           return of(null);
  //         })
  //       )
  //       .subscribe(response => {
  //         if (response) {
  //           localStorage.removeItem('token');
  //           this.toastr.success('Account deleted successfully');
  //           this.router.navigate(['/']);
  //         }
  //       });
  //   }
  // }
}
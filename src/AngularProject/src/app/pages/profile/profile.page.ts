import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { NavigationComponent } from '../../components/navigation/navigation.component';
import { FooterComponent } from '../../components/footer/footer.component';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-profile.page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, NavigationComponent, FooterComponent],
  templateUrl: './profile.page.html',
  styleUrl: './profile.page.scss'
})
export class ProfilePage implements OnInit {
  user: any;
  showEditForm = false;
  showSettingsForm = false;
  editForm!: FormGroup;
  settingsForm!: FormGroup;

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.userService.getCurrentUser().subscribe((res) => {
      this.user = res?.data?.user;
      console.log(res);
      this.initForms();
    });
  }

  initForms() {
    this.editForm = this.fb.group({
      username: [this.user?.username || ''],
      firstName: [this.user?.firstName || ''],
      lastName: [this.user?.lastName || ''],
      email: [this.user?.email || ''],
      phoneNumber: [this.user?.phoneNumber || ''],
      weight: [this.user?.weight || ''],
      height: [this.user?.height || ''],
      sex: [this.user?.sex || '']
    });

    this.settingsForm = this.fb.group({
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$')
      ]]
    });
  }

  toggleEditForm() {
    this.showEditForm = !this.showEditForm;
    if (this.showEditForm) this.showSettingsForm = false;
  }

  toggleSettingsForm() {
    this.showSettingsForm = !this.showSettingsForm;
    if (this.showSettingsForm) this.showEditForm = false;
  }

  onSaveProfile() {
    const raw = this.editForm.value;
    const cleaned: any = {};
    for (const key in raw) {
      if (raw[key] !== null && raw[key] !== '' && raw[key].toString().trim() !== '') {
        cleaned[key] = raw[key];
      }
    }
    this.userService.updateUser(this.user.id, cleaned).subscribe(() => {
      this.showEditForm = false;
      this.ngOnInit();
      this.toastr.success('Profile updated successfully!');
    });
  }

  onResetPassword() {
    if (this.settingsForm.invalid) return;

    const newPassword = this.settingsForm.value.newPassword;
    if (newPassword) {
      this.userService.resetPassword(this.user.id, newPassword).subscribe(() => {
        this.showSettingsForm = false;
        this.toastr.success('Password reset successfully!');
      });
    }
  }

  // onDeleteAccount() {
  //   const confirmDelete = window.confirm('Are you sure you want to send a request to delete your account? This action is irreversible.'); // Are you sure you want to delete your account? This action is irreversible.
  //   if (confirmDelete) {
  //     this.userService.deleteUser(this.user.id).subscribe(() => {
  //       localStorage.removeItem('token');
  //       this.toastr.success('Account deleted.');
  //       window.location.href = '/';
  //     });
  //   }
  // }
}

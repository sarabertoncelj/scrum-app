import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { UserService } from 'src/app/services/user.service';
import { first } from 'rxjs/operators';
import { UserRole } from 'src/app/enums/UserRole';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-user-modal',
  templateUrl: './user-modal.component.html',
  styleUrls: ['./user-modal.component.css']
})
export class UserModalComponent implements OnInit {

  userForm: FormGroup;
  loading = false;
  submitted = false;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private service: UserService,
    ) {  }

  ngOnInit (): void {
    this.userForm = this.formBuilder.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      role: [UserRole.User, Validators.required],
      password: ['', [Validators.required, this.PasswordStrengthValidator]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.PasswordsEqualsValidator });
  }

  get f () { return this.userForm.controls; }

  PasswordsEqualsValidator(group: FormGroup) {
    let pass = group.get('password').value;
    let confirmPass = group.get('confirmPassword').value;
    return pass === confirmPass ? null : { notSame: true }   
  }

  PasswordStrengthValidator(control: FormControl) {
    let pass = control.value;
    
    let returnObj = {};
    if(pass.length < 8) {
      returnObj['tooShort'] =  true;
    }
    if(pass.toUpperCase() == pass) {
      returnObj['noLowerCase'] =  true;
    }
    if(pass.toLowerCase() == pass) {
      returnObj['noUpperCase'] =  true;
    }
    if(!pass.match(/[_\W0-9]/) && !pass.match(/\d/g)) {
      returnObj['noSpecialCharacterOrDigit'] =  true;
    }

    if(Object.keys(returnObj).length > 0) {
      return returnObj;
    } else {
      return null;
    }
  }

  onSubmit () {
    this.submitted = true;
    
    // stop here if form is invalid
    if (this.userForm.invalid) {
      return;
    }

    this.loading = true;
    this.service.register(this.userForm.value)
      .pipe(first())
      .subscribe(
        data => {
          this.activeModal.close(data);
        },
        err => {
          this.activeModal.dismiss(err.error.message)
        }
      );
  }


}

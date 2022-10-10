import { Router } from '@angular/router';
import { AuthenticationService} from '../../services/authentication.service';
import { Component, OnInit } from '@angular/core';
import { User } from '../../models/User';
import { first } from 'rxjs/operators';
import { UserService } from '../../services/user.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertService } from 'src/app/services/alert.service';

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private router: Router,
    private alertService: AlertService
  ) {
    
   }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
    //this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }
  get f() { return this.loginForm.controls; }
  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
        return;
    }
    
    this.loading = true;
    this.authenticationService.login(this.f.username.value, this.f.password.value).subscribe(
      data => {
        this.alertService.info("Successfully logged in as "+ data['username'], {keepAfterRouteChange: true});
        this.router.navigate(['/projects']);                
      },
      error => {
        this.loading = false;
        this.alertService.error(error.error.message)
      });
  }
  
}

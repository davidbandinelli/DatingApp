import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  // accesso alla form
  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  // gestione evento abbandono finestra browser mentre l'edit è in corso
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private route: ActivatedRoute, private alertify: AlertifyService,
              private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    // il model arriva dal resolver
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotourl.subscribe(photoUrl => this.photoUrl = photoUrl);

  }

  updateUser() {
    // console.log(this.user);
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe(next => {
      this.alertify.success('Profile updated succesfully');
      this.editForm.reset(this.user);
    }, error => {
      this.alertify.error(error);
    });
  }

  // event handler proveniente dal child component (photo-editor) per intercettare il cambio di main photo
  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }

}

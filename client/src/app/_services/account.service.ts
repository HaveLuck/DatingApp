import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private currentuserSource = new ReplaySubject<User>(1); // trả về dữ liệu đầu tiên
  currentUser$ = this.currentuserSource.asObservable();

  
  constructor(private http: HttpClient) { }
  private modelUser: User;
  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
        }
      })
    )
  }
  // luu cookie
  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentuserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentuserSource.next();
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: any) => {
        const data = response;
        if (data) {
          localStorage.setItem('user', JSON.stringify(data));
          this.currentuserSource.next(data);
        }
      })
    )
  }
}

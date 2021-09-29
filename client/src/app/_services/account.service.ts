import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';
  private currentuserSource = new ReplaySubject<User>(1); // trả về dữ liệu đầu tiên
  currentUser$ = this.currentuserSource.asObservable();
  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentuserSource.next(user);
        }
      })
    )
  }
  setCurrentUser(user: User) {
    this.currentuserSource.next(user);
  }
  logout() {
    localStorage.removeItem('user');
    this.currentuserSource.next();
  }
  register(model: any) {
    return this.http.post(this.baseUrl + 'account/register', model).pipe(
      map((response: any)=>{
        const data = response;
        if(data){
          localStorage.setItem('user', JSON.stringify(data));
          this.currentuserSource.next(data);
        }
      })
    )
  }
}

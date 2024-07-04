# LAllermann
## TODO
### REST
 
- [X] Implement token refresh
### Website
#### Chapter 1: Login and Registration
- [X] Implement user registration page
- [X] UserAuthenticationService SignOut (even if not needed per se)
- [X] Implement UserAuthenticationService.RefreshTokenIfNeeded 
- [X] Implement that if the cookie is expired you are actually logged out (or check why it currently still isnt working)
- [X] Use HttpOnly Cookies and configure AuthProperties with usefull settings
- [X] Test if token needs multiple Roles (Like User and Admin) instead of Admins can automatically see User Content
- [X] lookup refresh token (Will not be used for now)
- [X] Lookup SignInManager
- [ ] Decide for How and When to refresh the token (Undecided until rough PasswordManager implementation)
- [ ] Implement refresh page returns to original page
- [ ] Codereview and Cleanup

#### Chapter 2: General Layout / Design
- [X] Design on a general layout
- [X] Implement a general layout
- [X] Implement Index Page
- [X] Change Login and Register to use the new layout
- [X] Implement Impressum


#### Chapter 3: PasswordManager
- [X] Design a Layout for the PasswordManager Page
- [X] Implement PasswordManager Page
- [ ] 

#### Chapter 4: Make it go online
- [ ] Stop all Development/Debug entpoints (Website)
- [ ] Stop all Development/Debug entpoints (API)
- [ ] Lookup what to do to go from Development to Production
- [ ] Lookup how to deploy to Ubuntu Server (Apache)
- [ ] Deploy API to Apache
- [ ] Deploy Website to Apache
- [ ] Test if everything works

#### Bugfixes
- [X] Bugfix: calling /refresh while not logged in throws a nullpointer exception

#### Optional
- [ ] Implement a User Panel
- [ ] Implement a Admin Panel
- [ ] Think about a better way to refresh the token in InteractiveServer Rendermode

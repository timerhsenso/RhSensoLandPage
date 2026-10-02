const btn=document.querySelector('.menu-btn'),menu=document.querySelector('.menu');
btn?.addEventListener('click',()=>{menu.classList.toggle('open');btn.setAttribute('aria-expanded',menu.classList.contains('open'))});
document.querySelectorAll('.menu a').forEach(a=>a.addEventListener('click',()=>menu.classList.remove('open')));
document.getElementById('year').textContent=new Date().getFullYear();
const io=new IntersectionObserver(es=>es.forEach(e=>{if(e.isIntersecting)e.target.classList.add('visible')}),{threshold:.12});
document.querySelectorAll('.reveal').forEach(el=>io.observe(el));
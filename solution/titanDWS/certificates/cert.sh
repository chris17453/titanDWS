rm cer.key -f
rm cer.csr -f
rm keyStore.p12 -f
rm cer.pem
rm cer.pfx

#generate key
openssl genrsa -out cer.key 2048
#Generate csr
openssl req -new -key ./cer.key -out cer.csr
#generate p12
openssl pkcs12 -export -out keyStore.p12 -inkey ./cer.key -in ./cer.csr 
#Generate self signed cert
openssl x509 -in cer.csr -out cer.pem -req -signkey cer.key -days 7300
#Clip it into a chain file
cat cer.key>>cer.pem
#GENERATE PFX
openssl pkcs12 -export -out cer.pfx -inkey cer.key -in cer.pem

